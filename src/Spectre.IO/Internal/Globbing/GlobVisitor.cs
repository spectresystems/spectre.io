using System.Collections.Generic;

namespace Spectre.IO.Internal
{
    internal sealed class GlobVisitor
    {
        private readonly IFileSystem _fileSystem;
        private readonly IEnvironment _environment;

        public GlobVisitor(IFileSystem fileSystem, IEnvironment environment)
        {
            _fileSystem = fileSystem;
            _environment = environment;
        }

        public IEnumerable<IFileSystemInfo> Walk(GlobNode node, GlobberSettings settings)
        {
            var context = new GlobVisitorContext(_environment, settings);
            node.Accept(this, context);
            return context.Results;
        }

        public void VisitRecursiveWildcardSegment(RecursiveWildcardNode node, GlobVisitorContext context)
        {
            var path = _fileSystem.GetDirectory(context.Path);
            if (_fileSystem.Exist(path.Path))
            {
                // Check if folders match.
                var candidates = new List<IFileSystemInfo>();
                candidates.Add(path);
                candidates.AddRange(FindCandidates(path.Path, node, context, SearchScope.Recursive, includeFiles: false));

                foreach (var candidate in candidates)
                {
                    var pushed = false;
                    if (context.Path?.FullPath != candidate.Path.FullPath)
                    {
                        context.Push(candidate.Path.FullPath.Substring(path.Path.FullPath.Length + (path.Path.FullPath.Length > 1 ? 1 : 0)));
                        pushed = true;
                    }

                    if (node.Next != null)
                    {
                        node.Next.Accept(this, context);
                    }
                    else
                    {
                        context.AddResult(candidate);
                    }

                    if (pushed)
                    {
                        context.Pop();
                    }
                }
            }
        }

        public void VisitRelativeRoot(RelativeRootNode node, GlobVisitorContext context)
        {
            // Push each path to the context.
            var pushedSegmentCount = 0;
            foreach (var segment in context.Root.Segments)
            {
                context.Push(segment);
                pushedSegmentCount++;
            }

            node.Next?.Accept(this, context);

            // Pop all segments we added to the context.
            for (var index = 0; index < pushedSegmentCount; index++)
            {
                context.Pop();
            }
        }

        public void VisitSegment(PathNode node, GlobVisitorContext context)
        {
            if (node.IsIdentifier)
            {
                // Get the (relative) path to the current node.
                var segment = node.GetPath();

                // Get a directory that matches this segment.
                // This might be a file but we can't be sure so we need to check.
                var directoryPath = context.Path.Combine(segment);
                var directory = _fileSystem.GetDirectory(directoryPath);

                // Should we not traverse this directory?
                if (directory.Exists && !context.ShouldTraverse(directory))
                {
                    return;
                }

                if (node.Next == null)
                {
                    if (directory.Exists)
                    {
                        // Directory
                        context.AddResult(directory);
                    }
                    else
                    {
                        // Then it must be a file (if it exist).
                        var filePath = context.Path.CombineWithFilePath(new FilePath(segment));
                        var file = _fileSystem.File.Retrieve(filePath);
                        if (file.Exists && context.ShouldInclude(file))
                        {
                            // File
                            context.AddResult(file);
                        }
                    }
                }
                else
                {
                    // Push the current node to the context.
                    context.Push(node.GetPath());
                    node.Next.Accept(this, context);
                    context.Pop();
                }
            }
            else
            {
                if (node.Segments.Count > 1)
                {
                    var path = _fileSystem.GetDirectory(context.Path);
                    if (path.Exists)
                    {
                        foreach (var candidate in FindCandidates(path.Path, node, context, SearchScope.Current))
                        {
                            if (node.Next != null)
                            {
                                context.Push(candidate.Path.FullPath.Substring(path.Path.FullPath.Length + (path.Path.FullPath.Length > 1 ? 1 : 0)));
                                node.Next.Accept(this, context);
                                context.Pop();
                            }
                            else
                            {
                                context.AddResult(candidate);
                            }
                        }
                    }
                }
            }
        }

        public void VisitHomeDirectory(HomeDirectoryNode node, GlobVisitorContext context)
        {
            if (node.Next != null)
            {
                context.Push(_environment.HomeDirectory.FullPath);
                node.Next.Accept(this, context);
                context.Pop();
            }
        }

        public void VisitUnixRoot(UnixRootNode node, GlobVisitorContext context)
        {
            if (node.Next != null)
            {
                context.Push("/");
                node.Next.Accept(this, context);
                context.Pop();
            }
        }

        public void VisitUncRoot(UncRootNode node, GlobVisitorContext context)
        {
            if (node.Next != null)
            {
                context.Push($@"\\{node.Server}");
                node.Next.Accept(this, context);
                context.Pop();
            }
        }

        public void VisitWildcardSegmentNode(WildcardNode node, GlobVisitorContext context)
        {
            var path = _fileSystem.GetDirectory(context.Path);
            if (_fileSystem.Exist(path.Path))
            {
                foreach (var candidate in FindCandidates(path.Path, node, context, SearchScope.Current))
                {
                    context.Push(candidate.Path.FullPath.Substring(path.Path.FullPath.Length + (path.Path.FullPath.Length > 1 ? 1 : 0)));
                    if (node.Next != null)
                    {
                        node.Next.Accept(this, context);
                    }
                    else
                    {
                        context.AddResult(candidate);
                    }

                    context.Pop();
                }
            }
        }

        public void VisitWindowsRoot(WindowsRootNode node, GlobVisitorContext context)
        {
            if (node.Next != null)
            {
                context.Push(node.Drive + ":");
                node.Next.Accept(this, context);
                context.Pop();
            }
        }

        public void VisitParent(ParentDirectoryNode node, GlobVisitorContext context)
        {
            if (node.Next != null)
            {
                // Back up one level.
                var last = context.Pop();
                node.Next.Accept(this, context);

                // Push the segment back so pop/push
                // count remains balanced.
                context.Push(last);
            }
        }

        public void VisitCurrent(CurrentDirectoryNode node, GlobVisitorContext context)
        {
            node.Next?.Accept(this, context);
        }

        private IEnumerable<IFileSystemInfo> FindCandidates(
            DirectoryPath path,
            MatchableNode node,
            GlobVisitorContext context,
            SearchScope option,
            bool includeFiles = true,
            bool includeDirectories = true)
        {
            var result = new List<IFileSystemInfo>();
            var current = _fileSystem.GetDirectory(path);

            // Directories
            if (includeDirectories)
            {
                foreach (var directory in current.GetDirectories("*", option))
                {
                    var lastPath = directory.Path.Segments[directory.Path.Segments.Count - 1];
                    if (node.IsMatch(lastPath) && context.ShouldTraverse(directory))
                    {
                        result.Add(directory);
                    }
                }
            }

            // Files
            if (includeFiles)
            {
                foreach (var file in current.GetFiles("*", option))
                {
                    var lastPath = file.Path.Segments[file.Path.Segments.Count - 1];
                    if (node.IsMatch(lastPath) && context.ShouldInclude(file))
                    {
                        result.Add(file);
                    }
                }
            }

            return result;
        }
    }
}
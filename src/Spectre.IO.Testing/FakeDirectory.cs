using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Spectre.IO.Testing
{
    /// <summary>
    /// Represents a fake directory.
    /// </summary>
    [DebuggerDisplay("{Path,nq}")]
    public sealed class FakeDirectory : IDirectory
    {
        private readonly FakeFileSystemTree _tree;

        /// <inheritdoc/>
        public bool Exists { get; internal set; }

        /// <inheritdoc/>
        public bool Hidden { get; internal set; }

        /// <summary>
        /// Gets the path to the directory.
        /// </summary>
        /// <value>The path.</value>
        public DirectoryPath Path { get; }

        /// <inheritdoc/>
        Path IFileSystemInfo.Path => Path;

        internal FakeDirectory? Parent { get; set; }

        internal FakeDirectoryContent Content { get; }

        internal FakeDirectory(FakeFileSystemTree tree, DirectoryPath path)
        {
            _tree = tree;
            Path = path;
            Content = new FakeDirectoryContent(this, tree.Comparer);
        }

        /// <inheritdoc/>
        public void Create()
        {
            _tree.CreateDirectory(this);
        }

        /// <inheritdoc/>
        public IDirectory Move(DirectoryPath destination)
        {
            if (destination is null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            _tree.MoveDirectory(this, destination);
            return _tree.FindDirectory(destination) ?? new FakeDirectory(_tree, destination);
        }

        /// <inheritdoc/>
        public void Delete(bool recursive)
        {
            _tree.DeleteDirectory(this, recursive);
        }

        /// <inheritdoc/>
        public IEnumerable<IDirectory> GetDirectories(string filter, SearchScope scope)
        {
            var result = new List<IDirectory>();
            var stack = new Stack<FakeDirectory>();
            foreach (var child in Content.Directories.Values)
            {
                if (child.Exists)
                {
                    stack.Push(child);
                }
            }

            // Rewrite the filter to a regex expression.
            var expression = CreateRegex(filter);

            while (stack.Count > 0)
            {
                // Pop a directory from the stack.
                var current = stack.Pop();

                // Is this a match? In that case, add it to the result.
                if (expression.IsMatch(current.Path.GetDirectoryName()))
                {
                    result.Add(current);
                }

                // Recurse?
                if (scope == SearchScope.Recursive)
                {
                    foreach (var child in current.Content.Directories.Values)
                    {
                        if (child.Exists)
                        {
                            stack.Push(child);
                        }
                    }
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public IEnumerable<IFile> GetFiles(string filter, SearchScope scope)
        {
            var result = new List<IFile>();
            var stack = new Stack<FakeDirectory>();

            // Rewrite the filter to a regex expression.
            var expression = CreateRegex(filter);

            // Just interested in this directory?
            if (scope == SearchScope.Current)
            {
                return GetFiles(this, expression);
            }

            stack.Push(this);
            while (stack.Count > 0)
            {
                // Pop a directory from the stack.
                var current = stack.Pop();

                // Add all files we can find.
                result.AddRange(GetFiles(current, expression));

                // Recurse?
                if (scope == SearchScope.Recursive)
                {
                    foreach (var child in current.Content.Directories.Values)
                    {
                        if (child.Exists)
                        {
                            stack.Push(child);
                        }
                    }
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public void Refresh()
        {
        }

        private static IEnumerable<FakeFile> GetFiles(FakeDirectory current, Regex expression)
        {
            var result = new List<FakeFile>();
            foreach (var file in current.Content.Files)
            {
                if (file.Value.Exists)
                {
                    // Is this a match? In that case, add it to the result.
                    if (expression.IsMatch(file.Key.GetFilename().FullPath))
                    {
                        result.Add(current.Content.Files[file.Key]);
                    }
                }
            }

            return result;
        }

        private static Regex CreateRegex(string pattern)
        {
            pattern = pattern.Replace(".", "\\.").Replace("*", ".*").Replace("?", ".{1}");
            return new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled);
        }
    }
}
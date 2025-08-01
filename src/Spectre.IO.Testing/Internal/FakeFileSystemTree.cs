using System.Globalization;

namespace Spectre.IO.Testing;

internal sealed class FakeFileSystemTree
{
    private readonly FakeDirectory _root;

    public IPathComparer Comparer { get; }

    public FakeFileSystemTree(IEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(environment);

        if (environment.WorkingDirectory == null)
        {
            throw new ArgumentException("Working directory not set.");
        }

        if (environment.WorkingDirectory.IsRelative)
        {
            throw new ArgumentException("Working directory cannot be relative.");
        }

        Comparer = new PathComparer(environment.Platform.IsUnix());

        _root = new FakeDirectory(this, "/") { Exists = true };
        _root.Create();
    }

    public FakeDirectory CreateDirectory(DirectoryPath path)
    {
        return CreateDirectory(new FakeDirectory(this, path));
    }

    public FakeDirectory CreateDirectory(FakeDirectory directory)
    {
        var path = directory.Path;
        var queue = new Queue<string>(path.Segments);

        FakeDirectory? current = null;
        var children = _root.Content;

        while (queue.Count > 0)
        {
            // Get the segment.
            var currentSegment = queue.Dequeue();
            var parent = current;

            // Calculate the current path.
            path = parent != null ? parent.Path.Combine(currentSegment) : new DirectoryPath(currentSegment);

            if (!children.Directories.TryGetValue(path, out var childDirectory))
            {
                current = queue.Count == 0 ? directory : new FakeDirectory(this, path);
                current.Parent = parent ?? _root;
                current.Hidden = false;
                children.Add(current);
            }
            else
            {
                current = childDirectory;
            }

            current.Exists = true;
            children = current.Content;
        }

        return directory;
    }

    public void CreateFile(FakeFile file)
    {
        // Get the directory that the file exists in.
        var directory = FindDirectory(file.Path.GetDirectory());
        if (directory == null)
        {
            file.Exists = false;
            throw new DirectoryNotFoundException(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Could not find a part of the path '{0}'.",
                    file.Path.FullPath));
        }

        if (!directory.Content.Files.ContainsKey(file.Path))
        {
            // Add the file to the directory.
            file.Exists = true;
            directory.Content.Add(file);
        }
    }

    public void DeleteDirectory(FakeDirectory fakeDirectory, bool recursive)
    {
        var root = new Stack<FakeDirectory>();
        var result = new Stack<FakeDirectory>();

        if (fakeDirectory.Exists)
        {
            root.Push(fakeDirectory);
        }

        while (root.Count > 0)
        {
            var node = root.Pop();
            result.Push(node);

            var directories = node.Content.Directories;

            if (directories.Count > 0 && !recursive)
            {
                throw new IOException("The directory is not empty.");
            }

            foreach (var child in directories)
            {
                root.Push(child.Value);
            }
        }

        while (result.Count > 0)
        {
            var directory = result.Pop();

            var files = directory.Content.Files.Select(x => x).ToArray();
            if (files.Length > 0 && !recursive)
            {
                throw new IOException("The directory is not empty.");
            }

            foreach (var file in files)
            {
                // Delete the file.
                DeleteFile(file.Value);
            }

            // Remove the directory from the parent.
            directory.Parent?.Content.Remove(directory);

            // Mark the directory as it doesn't exist.
            directory.Exists = false;
        }
    }

    public bool DeleteFile(FakeFile file)
    {
        if (!file.Exists)
        {
            throw new FileNotFoundException("File does not exist.", file.Path.FullPath);
        }

        if ((file.Attributes & FileAttributes.ReadOnly) != 0)
        {
            throw new IOException($"Cannot delete readonly file '{file.Path.FullPath}'.");
        }

        var result = false;

        // Find the directory.
        var directory = FindDirectory(file.Path.GetDirectory());
        if (directory != null)
        {
            // Remove the file from the directory.
            directory.Content.Remove(file);
            result = true;
        }

        // Reset all properties.
        file.Exists = false;
        file.Content = new byte[4096];
        file.ContentLength = 0;

        return result;
    }

    public FakeDirectory? FindDirectory(DirectoryPath path)
    {
        // Want the root?
        if (path.Segments.Count == 0)
        {
            return _root;
        }

        var queue = new Queue<string>(path.Segments);

        FakeDirectory? current = null;
        var children = _root.Content;

        while (queue.Count > 0)
        {
            // Set the parent.
            var parent = current;

            // Calculate the current path.
            var segment = queue.Dequeue();
            path = parent != null ? parent.Path.Combine(segment) : new DirectoryPath(segment);

            // Find the current path.
            if (!children.Directories.TryGetValue(path, out var directory))
            {
                return null;
            }

            current = directory;
            children = current.Content;
        }

        return current;
    }

    public FakeFile? FindFile(FilePath path)
    {
        var directory = FindDirectory(path.GetDirectory());
        if (directory != null && directory.Content.Files.TryGetValue(path, out var file))
        {
            return file;
        }

        return null;
    }

    public void CopyFile(FakeFile file, FilePath destination, bool overwrite)
    {
        if (!file.Exists)
        {
            throw new FileNotFoundException("File does not exist.");
        }

        // Already exists?
        var destinationFile = FindFile(destination);
        if (destinationFile != null)
        {
            if (!overwrite)
            {
                throw new IOException(
                    $"File '{destination.FullPath}' exists and overwrite is set to false.");
            }
        }

        // Directory exists?
        var directory = FindDirectory(destination.GetDirectory());
        if (directory?.Exists != true)
        {
            throw new DirectoryNotFoundException(
                $"The destination path '{destination.FullPath}' does not exist.");
        }

        // Make sure the file exist.
        destinationFile ??= new FakeFile(this, destination);

        // Copy the data from the original file to the destination.
        using (var input = file.OpenRead())
        using (var output = destinationFile.OpenWrite())
        {
            input.CopyTo(output);
        }
    }

    public void CreateSymbolicLink(FakeFile file, FilePath destination)
    {
        if (!file.Exists)
        {
            throw new FileNotFoundException("File does not exist.");
        }

        // Already exists?
        var destinationFile = FindFile(destination);
        if (destinationFile?.Exists == true)
        {
            throw new IOException(
                $"The destination path '{destination.FullPath}' already exists. Cannot create symbolic link.");
        }

        // Directory exists?
        var directory = FindDirectory(destination.GetDirectory());
        if (directory?.Exists != true)
        {
            throw new DirectoryNotFoundException(
                $"The destination path '{destination.FullPath}' does not exist.");
        }

        // Make sure the file exist.
        if (destinationFile == null)
        {
            directory.Content.Add(new FakeFile(this, destination)
            {
                Exists = true, SymbolicLink = file, Attributes = FileAttributes.ReparsePoint,
            });
        }
    }

    public void MoveFile(FakeFile fakeFile, FilePath destination, bool overwrite = false)
    {
        // Copy the file to the new location.
        CopyFile(fakeFile, destination, overwrite);

        // Delete the original file.
        fakeFile.Delete();
    }

    public void MoveDirectory(FakeDirectory fakeDirectory, DirectoryPath destination)
    {
        var root = new Stack<FakeDirectory>();
        var result = new Stack<FakeDirectory>();

        if (string.IsNullOrEmpty(destination.FullPath))
        {
            throw new ArgumentException("The destination directory is empty.");
        }

        if (fakeDirectory.Path.Equals(destination))
        {
            throw new IOException("The directory being moved and the destination directory have the same name.");
        }

        if (FindDirectory(destination) != null)
        {
            throw new IOException("The destination directory already exists.");
        }

        var destinationParentPathStr =
            string.Join("/", destination.Segments.Take(destination.Segments.Count - 1).DefaultIfEmpty("/"));

        var destinationParentPath =
            new DirectoryPath(string.IsNullOrEmpty(destinationParentPathStr) ? "/" : destinationParentPathStr);

        if (FindDirectory(destinationParentPath) == null)
        {
            throw new DirectoryNotFoundException("The parent destination directory " + destinationParentPath.FullPath +
                                                 " could not be found.");
        }

        if (fakeDirectory.Exists)
        {
            root.Push(fakeDirectory);
        }

        // Create destination directories and move files
        while (root.Count > 0)
        {
            var node = root.Pop();
            result.Push(node);

            // Create destination directory
            var relativePath = fakeDirectory.Path.GetRelativePath(node.Path);
            var destinationPath = destination.Combine(relativePath);
            CreateDirectory(destinationPath);

            var files = node.Content.Files.Select(x => x).ToArray();
            foreach (var file in files)
            {
                // Move the file.
                MoveFile(file.Value, destinationPath.CombineWithFilePath(file.Key.GetFilename()));
            }

            var directories = node.Content.Directories;
            foreach (var child in directories)
            {
                root.Push(child.Value);
            }
        }

        // Delete source directories
        while (result.Count > 0)
        {
            var directory = result.Pop();

            // Delete the directory.
            directory.Parent?.Content.Remove(directory);

            // Mark the directory as it doesn't exist.
            directory.Exists = false;
        }
    }

    public override string ToString()
    {
        var builder = new IndentedStringBuilder();
        foreach (var child in _root.GetDirectories("*", SearchScope.Current))
        {
            Dump(builder, child);
        }

        return builder.ToString().Trim();
    }

    private static void Dump(IndentedStringBuilder writer, FakeDirectory directory)
    {
        writer.AppendLine(directory.Path.GetDirectoryName());

        using (writer.Indent())
        {
            var children = directory.GetDirectories("*", SearchScope.Current);
            if (children.Count == 0)
            {
                var files = directory.GetFiles("*", SearchScope.Current);
                foreach (var file in files)
                {
                    writer.AppendLine(file.Path.GetFilename().FullPath);
                }
            }
            else
            {
                foreach (var child in children)
                {
                    Dump(writer, child);
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Spectre.IO.Internal;

internal sealed class Directory : IDirectory
{
    private readonly DirectoryInfo _directory;

    public DirectoryPath Path { get; }

    Path IFileSystemInfo.Path => Path;

    public bool Exists => _directory.Exists;

    public bool Hidden => (_directory.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden;

    public DateTime LastWriteTime => _directory.LastWriteTime;

    public Directory(DirectoryPath path)
    {
        Path = path;
        _directory = new DirectoryInfo(Path.FullPath);
    }

    public void Create()
    {
        _directory.Create();
    }

    public IDirectory Move(DirectoryPath destination)
    {
        if (destination == null)
        {
            throw new ArgumentNullException(nameof(destination));
        }

        _directory.MoveTo(destination.FullPath);
        return new Directory(destination.FullPath);
    }

    public void Delete(bool recursive)
    {
        _directory.Delete(recursive);
    }

    public void Refresh()
    {
        _directory.Refresh();
    }

    public IEnumerable<IDirectory> GetDirectories(string filter, SearchScope scope)
    {
        var option = scope == SearchScope.Current ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories;
        return _directory.GetDirectories(filter, option)
            .Select(directory => new Directory(directory.FullName));
    }

    public IEnumerable<IFile> GetFiles(string filter, SearchScope scope)
    {
        var option = scope == SearchScope.Current ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories;
        return _directory.GetFiles(filter, option)
            .Select(file => new File(new FilePath(file.FullName)));
    }
}
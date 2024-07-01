using System;
using System.Collections.Generic;
using System.Linq;

namespace Spectre.IO.Internal;

internal sealed class GlobVisitorContext
{
    private readonly GlobberSettings _settings;
    private readonly List<string> _pathParts;

    public DirectoryPath Root { get; set; }
    public List<IFileSystemInfo> Results { get; }

    internal DirectoryPath Path { get; private set; }

    public GlobVisitorContext(
        IEnvironment environment,
        GlobberSettings settings)
    {
        if (environment is null)
        {
            throw new ArgumentNullException(nameof(environment));
        }

        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _pathParts = new List<string>();

        Results = new List<IFileSystemInfo>();

        Root = _settings.Root ?? environment.WorkingDirectory;
        Root = Root.MakeAbsolute(environment);

        Path = new DirectoryPath(string.Empty);
    }

    public void AddResult(IFileSystemInfo path)
    {
        Results.Add(path);
    }

    public void Push(string path)
    {
        _pathParts.Add(path);
        Path = GenerateFullPath();
    }

    public string Pop()
    {
        var last = _pathParts[_pathParts.Count - 1];
        _pathParts.RemoveAt(_pathParts.Count - 1);
        Path = GenerateFullPath();
        return last;
    }

    public bool ShouldTraverse(IDirectory info)
    {
        return _settings.Predicate == null || _settings.Predicate(info);
    }

    public bool ShouldInclude(IFile file)
    {
        return _settings.FilePredicate == null || _settings.FilePredicate(file);
    }

    private DirectoryPath GenerateFullPath()
    {
        if (_pathParts.Count > 0 && _pathParts[0] == @"\\")
        {
            // UNC path
            var path = string.Concat(@"\\", string.Join(@"\", _pathParts.Skip(1)));
            return new DirectoryPath(path);
        }

        if (_pathParts.Count > 0 && _pathParts[0] == "/")
        {
            // Unix root path
            var path = string.Concat("/", string.Join("/", _pathParts.Skip(1)));
            return new DirectoryPath(path);
        }
        else
        {
            // Regular path
            var path = string.Join("/", _pathParts);
            if (string.IsNullOrWhiteSpace(path))
            {
                path = "./";
            }

            return new DirectoryPath(path);
        }
    }
}
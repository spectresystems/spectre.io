﻿using System.Collections.Generic;

namespace Spectre.IO.Testing;

internal sealed class FakeDirectoryContent
{
    private readonly Dictionary<DirectoryPath, FakeDirectory> _directories;
    private readonly Dictionary<FilePath, FakeFile> _files;

    public FakeDirectory Owner { get; }

    public IReadOnlyDictionary<DirectoryPath, FakeDirectory> Directories => _directories;

    public IReadOnlyDictionary<FilePath, FakeFile> Files => _files;

    public FakeDirectoryContent(FakeDirectory owner, IPathComparer comparer)
    {
        Owner = owner;
        _directories = new Dictionary<DirectoryPath, FakeDirectory>(comparer);
        _files = new Dictionary<FilePath, FakeFile>(comparer);
    }

    public void Add(FakeDirectory directory)
    {
        _directories.Add(directory.Path, directory);
    }

    public void Add(FakeFile file)
    {
        _files.Add(file.Path, file);
    }

    public void Remove(FakeDirectory directory)
    {
        _directories.Remove(directory.Path);
    }

    public void Remove(FakeFile file)
    {
        _files.Remove(file.Path);
    }
}
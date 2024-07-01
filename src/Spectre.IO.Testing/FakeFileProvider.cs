namespace Spectre.IO.Testing;

internal sealed class FakeFileProvider : IFileProvider
{
    private readonly FakeFileSystemTree _tree;

    internal FakeFileProvider(FakeFileSystemTree tree)
    {
        _tree = tree;
    }

    IFile IFileProvider.Retrieve(FilePath path)
    {
        return Get(path);
    }

    public FakeFile Get(FilePath path)
    {
        return _tree.FindFile(path) ?? new FakeFile(_tree, path);
    }
}
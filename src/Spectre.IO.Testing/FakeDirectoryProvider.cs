namespace Spectre.IO.Testing
{
    internal sealed class FakeDirectoryProvider : IDirectoryProvider
    {
        private readonly FakeFileSystemTree _tree;

        internal FakeDirectoryProvider(FakeFileSystemTree tree)
        {
            _tree = tree;
        }

        IDirectory IDirectoryProvider.Retrieve(DirectoryPath path)
        {
            return Get(path);
        }

        public FakeDirectory Get(DirectoryPath path)
        {
            return _tree.FindDirectory(path) ?? new FakeDirectory(_tree, path);
        }
    }
}

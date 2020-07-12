using Spectre.IO.Internal;

namespace Spectre.IO
{
    internal sealed class DirectoryProvider : IDirectoryProvider
    {
        public IDirectory Retrieve(DirectoryPath path)
        {
            return new Directory(path);
        }
    }
}
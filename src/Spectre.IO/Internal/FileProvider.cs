using Spectre.IO.Internal;

namespace Spectre.IO;

internal sealed class FileProvider : IFileProvider
{
    public IFile Retrieve(FilePath path)
    {
        return new File(path);
    }
}
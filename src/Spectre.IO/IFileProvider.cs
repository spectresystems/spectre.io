namespace Spectre.IO;

/// <summary>
/// Represents a file provider.
/// </summary>
[PublicAPI]
public interface IFileProvider
{
    /// <summary>
    /// Gets a <see cref="IFile"/> instance.
    /// </summary>
    /// <param name="path">The file path.</param>
    /// <returns>An <see cref="IFile"/> instance.</returns>
    IFile Retrieve(FilePath path);
}
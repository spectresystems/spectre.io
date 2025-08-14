namespace Spectre.IO;

/// <summary>
/// Represents a file system.
/// </summary>
[PublicAPI]
public interface IFileSystem
{
    /// <summary>
    /// Gets the file system's <see cref="IPathComparer"/>.
    /// </summary>
    IPathComparer Comparer { get; }

    /// <summary>
    /// Gets the <see cref="IFileProvider"/> belonging to this <see cref="IFileSystem"/>>.
    /// </summary>
    /// <returns>An <see cref="IFileProvider"/> instance.</returns>
    IFileProvider File { get; }

    /// <summary>
    /// Gets the <see cref="IDirectoryProvider"/> belonging to this <see cref="IFileSystem"/>>.
    /// </summary>
    /// <returns>An <see cref="IDirectoryProvider"/> instance.</returns>
    IDirectoryProvider Directory { get; }

    /// <summary>
    /// Creates a uniquely named, zero-byte temporary file on disk and returns the full path of that file.
    /// </summary>
    /// <param name="environment">The environment.</param>
    /// <returns>The created temporary file.</returns>
    IFile GetTempFile(IEnvironment environment);
}
namespace Spectre.IO;

/// <summary>
/// Represents a file system.
/// </summary>
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
}
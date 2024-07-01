using System;
using System.IO;

namespace Spectre.IO;

/// <summary>
/// Represents a file.
/// </summary>
public interface IFile : IFileSystemInfo
{
    /// <summary>
    /// Gets the path to the file.
    /// </summary>
    /// <value>The path.</value>
    new FilePath Path { get; }

    /// <summary>
    /// Gets the length of the file.
    /// </summary>
    /// <value>The length of the file.</value>
    long Length { get; }

    /// <summary>
    /// Gets or sets the file attributes.
    /// </summary>
    /// <value>The file attributes.</value>
    FileAttributes Attributes { get; set; }

    /// <summary>
    /// Gets the last write time.
    /// </summary>
    public DateTime LastWriteTime { get; }

    /// <summary>
    /// Copies the file to the specified destination path.
    /// </summary>
    /// <param name="destination">The destination path.</param>
    /// <returns>A <see cref="IFile"/> representing the copied file.</returns>
    IFile Copy(FilePath destination) => Copy(destination, false);

    /// <summary>
    /// Copies the file to the specified destination path.
    /// </summary>
    /// <param name="destination">The destination path.</param>
    /// <param name="overwrite">Will overwrite existing destination file if set to <c>true</c>.</param>
    /// <returns>A <see cref="IFile"/> representing the copied file.</returns>
    IFile Copy(FilePath destination, bool overwrite);

    /// <summary>
    /// Creates a symbolic link to the specified destination path.
    /// </summary>
    /// <param name="destination">The destination path.</param>
    void CreateSymbolicLink(FilePath destination);

    /// <summary>
    /// Moves the file to the specified destination path.
    /// </summary>
    /// <param name="destination">The destination path.</param>
    /// <returns>A <see cref="IFile"/> representing the moved file.</returns>
    IFile Move(FilePath destination) => Move(destination, false);

    /// <summary>
    /// Moves the file to the specified destination path.
    /// </summary>
    /// <param name="destination">The destination path.</param>
    /// <param name="overwrite">Will overwrite existing destination file if set to <c>true</c>.</param>
    /// <returns>A <see cref="IFile"/> representing the moved file.</returns>
    IFile Move(FilePath destination, bool overwrite);

    /// <summary>
    /// Deletes the file.
    /// </summary>
    void Delete();

    /// <summary>
    /// Opens the file using the specified options.
    /// </summary>
    /// <param name="fileMode">The file mode.</param>
    /// <param name="fileAccess">The file access.</param>
    /// <param name="fileShare">The file share.</param>
    /// <returns>A <see cref="Stream"/> to the file.</returns>
    Stream Open(FileMode fileMode, FileAccess fileAccess, FileShare fileShare);
}
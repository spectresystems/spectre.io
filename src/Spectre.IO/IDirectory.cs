using System;
using System.Collections.Generic;

namespace Spectre.IO;

/// <summary>
/// Represents a directory.
/// </summary>
public interface IDirectory : IFileSystemInfo
{
    /// <summary>
    /// Gets the path to the directory.
    /// </summary>
    /// <value>The path.</value>
    new DirectoryPath Path { get; }

    /// <summary>
    /// Gets the last write time.
    /// </summary>
    public DateTime LastWriteTime { get; }

    /// <summary>
    /// Creates the directory.
    /// </summary>
    void Create();

    /// <summary>
    /// Moves the directory to the specified destination path.
    /// </summary>
    /// <param name="destination">The destination path.</param>
    /// <returns>A <see cref="IDirectory"/> representing the moved directory.</returns>
    IDirectory Move(DirectoryPath destination);

    /// <summary>
    /// Deletes the directory.
    /// </summary>
    /// <param name="recursive">Will perform a recursive delete if set to <c>true</c>.</param>
    void Delete(bool recursive);

    /// <summary>
    /// Gets directories matching the specified filter and scope.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="scope">The search scope.</param>
    /// <returns>Directories matching the filter and scope.</returns>
    IEnumerable<IDirectory> GetDirectories(string filter, SearchScope scope);

    /// <summary>
    /// Gets files matching the specified filter and scope.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="scope">The search scope.</param>
    /// <returns>Files matching the specified filter and scope.</returns>
    IEnumerable<IFile> GetFiles(string filter, SearchScope scope);
}
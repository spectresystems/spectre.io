namespace Spectre.IO;

/// <summary>
/// Represents an entry in the file system.
/// </summary>
[PublicAPI]
public interface IFileSystemInfo
{
    /// <summary>
    /// Gets the path to the entry.
    /// </summary>
    /// <value>The path.</value>
    Path Path { get; }

    /// <summary>
    /// Gets a value indicating whether this <see cref="IFileSystemInfo"/> exists.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the entry exists; otherwise, <c>false</c>.
    /// </value>
    bool Exists { get; }

    /// <summary>
    /// Gets a value indicating whether this <see cref="IFileSystemInfo"/> is hidden.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the entry is hidden; otherwise, <c>false</c>.
    /// </value>
    bool Hidden { get; }

    /// <summary>
    /// Refreshes the state of the object.
    /// </summary>
    void Refresh();
}
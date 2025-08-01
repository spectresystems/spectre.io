using System.Text;

namespace Spectre.IO;

/// <summary>
/// Contains extensions for <see cref="IFileSystem"/>.
/// </summary>
[PublicAPI]
public static class IFileSystemExtensions
{
    /// <summary>
    /// Gets a <see cref="IFile"/> instance from the file system.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <param name="path">The path to the file.</param>
    /// <returns>An <see cref="IFile"/> instance representing the provided path.</returns>
    public static IFile GetFile(this IFileSystem fileSystem, FilePath path)
    {
        if (fileSystem is null)
        {
            throw new ArgumentNullException(nameof(fileSystem));
        }

        return fileSystem.File.Retrieve(path);
    }

    /// <summary>
    /// Gets a <see cref="IDirectory"/> instance from the file system.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <param name="path">The path to the directory.</param>
    /// <returns>An <see cref="IDirectory"/> instance representing the provided path.</returns>
    public static IDirectory GetDirectory(this IFileSystem fileSystem, DirectoryPath path)
    {
        if (fileSystem is null)
        {
            throw new ArgumentNullException(nameof(fileSystem));
        }

        return fileSystem.Directory.Retrieve(path);
    }

    /// <summary>
    /// Determines if a specified <see cref="FilePath"/> exist.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <param name="path">The path.</param>
    /// <returns>Whether or not the specified file exist.</returns>
    public static bool Exist(this IFileSystem fileSystem, FilePath path)
    {
        if (fileSystem == null)
        {
            throw new ArgumentNullException(nameof(fileSystem));
        }

        return GetFile(fileSystem, path).Exists;
    }

    /// <summary>
    /// Determines if a specified <see cref="DirectoryPath"/> exist.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <param name="path">The path.</param>
    /// <returns>Whether or not the specified directory exist.</returns>
    public static bool Exist(this IFileSystem fileSystem, DirectoryPath path)
    {
        if (fileSystem == null)
        {
            throw new ArgumentNullException(nameof(fileSystem));
        }

        return GetDirectory(fileSystem, path).Exists;
    }

    /// <summary>
    /// Opens a text file, reads all the text in the file, and then closes the file.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <param name="path">The file path to read from.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <returns>A string containing all text in the file.</returns>
    public static string ReadAllText(
        this IFileSystem fileSystem,
        FilePath path,
        Encoding? encoding = null)
    {
        ArgumentNullException.ThrowIfNull(fileSystem);
        ArgumentNullException.ThrowIfNull(path);

        var file = GetFile(fileSystem, path);
        using (var reader = new StreamReader(file.OpenRead(), encoding, true, -1, false))
        {
            return reader.ReadToEnd();
        }
    }

    /// <summary>
    /// Creates a new file, writes the specified string to the file, and then closes the file.
    /// If the target file already exists, it is overwritten.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <param name="path">The file path to write to.</param>
    /// <param name="contents">The string to write to the file.</param>
    /// <param name="encoding">The encoding to apply to the string.</param>
    public static void WriteAllText(
        this IFileSystem fileSystem,
        FilePath path,
        string contents,
        Encoding? encoding = null)
    {
        ArgumentNullException.ThrowIfNull(fileSystem);
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(contents);

        var file = GetFile(fileSystem, path);
        using (var writer = new StreamWriter(file.OpenWrite(), encoding, -1, false))
        {
            writer.Write(contents);
        }
    }
}
using System.Text;

namespace Spectre.IO;

/// <summary>
/// Contains extensions for <see cref="IFileProvider"/>.
/// </summary>
[PublicAPI]
public static class IFileProviderExtensions
{
    /// <summary>
    /// Gets whether or not the specified file exists.
    /// </summary>
    /// <param name="provider">The file provider.</param>
    /// <param name="path">The file path.</param>
    /// <returns><c>true</c> if the file exists; otherwise, <c>false</c>.</returns>
    public static bool Exists(this IFileProvider provider, FilePath path)
    {
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentNullException.ThrowIfNull(path);

        var file = provider.Retrieve(path);
        return file.Exists;
    }

    /// <summary>
    /// Gets the length of the file.
    /// </summary>
    /// <param name="provider">The file provider.</param>
    /// <param name="path">The file path.</param>
    /// <value>The length of the file.</value>
    /// <returns>The file size in bytes.</returns>
    public static long GetLength(this IFileProvider provider, FilePath path)
    {
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentNullException.ThrowIfNull(path);

        var file = provider.Retrieve(path);
        return file.Length;
    }

    /// <summary>
    /// Gets or sets the file attributes.
    /// </summary>
    /// <param name="provider">The file provider.</param>
    /// <param name="path">The file path.</param>
    /// <value>The file attributes.</value>
    /// <returns>The file attributes.</returns>
    public static FileAttributes GetAttributes(this IFileProvider provider, FilePath path)
    {
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentNullException.ThrowIfNull(path);

        var file = provider.Retrieve(path);
        return file.Attributes;
    }

    /// <summary>
    /// Gets or sets the file attributes.
    /// </summary>
    /// <param name="provider">The file provider.</param>
    /// <param name="path">The file path.</param>
    /// <param name="attributes">The file attributes.</param>
    /// <value>The file attributes.</value>
    public static void SetAttributes(this IFileProvider provider, FilePath path, FileAttributes attributes)
    {
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentNullException.ThrowIfNull(path);

        var file = provider.Retrieve(path);
        file.Attributes = attributes;
    }

    /// <summary>
    /// Copies the file to the specified destination path.
    /// </summary>
    /// <param name="provider">The file provider.</param>
    /// <param name="source">The source file path.</param>
    /// <param name="destination">The destination file path.</param>
    /// <param name="overwrite">Will overwrite existing destination file if set to <c>true</c>.</param>
    public static void Copy(
        this IFileProvider provider,
        FilePath source,
        FilePath destination,
        bool overwrite)
    {
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(destination);

        var file = provider.Retrieve(source);
        file.Copy(destination, overwrite);
    }

    /// <summary>
    /// Creates a symbolic link to the specified destination path.
    /// </summary>
    /// <param name="provider">The file provider.</param>
    /// <param name="source">The source file path.</param>
    /// <param name="destination">The destination path.</param>
    public static void CreateSymbolicLink(
        this IFileProvider provider,
        FilePath source,
        FilePath destination)
    {
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(destination);

        var file = provider.Retrieve(source);
        file.CreateSymbolicLink(destination);
    }

    /// <summary>
    /// Moves the file to the specified destination path.
    /// </summary>
    /// <param name="provider">The file provider.</param>
    /// <param name="source">The source file path.</param>
    /// <param name="destination">The destination file path.</param>
    public static void Move(this IFileProvider provider, FilePath source, FilePath destination)
    {
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(destination);

        Move(provider, source, destination, false);
    }

    /// <summary>
    /// Moves the file to the specified destination path.
    /// </summary>
    /// <param name="provider">The file provider.</param>
    /// <param name="source">The source file path.</param>
    /// <param name="destination">The destination file path.</param>
    /// <param name="overwrite">Will overwrite existing destination file if set to <c>true</c>.</param>
    public static void Move(this IFileProvider provider, FilePath source, FilePath destination, bool overwrite)
    {
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(destination);

        var file = provider.Retrieve(source);
        file.Move(destination, overwrite);
    }

    /// <summary>
    /// Deletes the specified file.
    /// </summary>
    /// <param name="provider">The file provider.</param>
    /// <param name="path">The file to delete.</param>
    public static void Delete(this IFileProvider provider, FilePath path)
    {
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentNullException.ThrowIfNull(path);

        var file = provider.Retrieve(path);
        file.Delete();
    }

    /// <summary>
    /// Opens the file using the specified options.
    /// </summary>
    /// <param name="provider">The file provider.</param>
    /// <param name="path">The file path.</param>
    /// <param name="fileMode">The file mode.</param>
    /// <param name="fileAccess">The file access.</param>
    /// <param name="fileShare">The file share.</param>
    /// <returns>A <see cref="Stream"/> to the file.</returns>
    public static Stream Open(
        this IFileProvider provider,
        FilePath path,
        FileMode fileMode,
        FileAccess fileAccess,
        FileShare fileShare)
    {
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentNullException.ThrowIfNull(path);

        var file = provider.Retrieve(path);
        return file.Open(fileMode, fileAccess, fileShare);
    }

    /// <summary>
    /// Opens the file using the specified options.
    /// </summary>
    /// <param name="provider">The file provider.</param>
    /// <param name="path">The file path to be opened.</param>
    /// <param name="mode">The mode.</param>
    /// <returns>A <see cref="Stream"/> to the file.</returns>
    public static Stream Open(this IFileProvider provider, FilePath path, FileMode mode)
    {
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentNullException.ThrowIfNull(path);

        return provider.Retrieve(path).Open(
            mode,
            mode == FileMode.Append ? FileAccess.Write : FileAccess.ReadWrite,
            FileShare.None);
    }

    /// <summary>
    /// Opens the file using the specified options.
    /// </summary>
    /// <param name="provider">The file provider.</param>
    /// <param name="path">The file path to be opened.</param>
    /// <param name="mode">The mode.</param>
    /// <param name="access">The access.</param>
    /// <returns>A <see cref="Stream"/> to the file.</returns>
    public static Stream Open(
        this IFileProvider provider,
        FilePath path,
        FileMode mode,
        FileAccess access)
    {
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentNullException.ThrowIfNull(path);

        return provider.Retrieve(path).Open(mode, access, FileShare.None);
    }

    /// <summary>
    /// Opens the file for reading.
    /// </summary>
    /// <param name="provider">The file provider.</param>
    /// <param name="path">The file path to be opened.</param>
    /// <returns>A <see cref="Stream"/> to the file.</returns>
    public static Stream OpenRead(this IFileProvider provider, FilePath path)
    {
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentNullException.ThrowIfNull(path);

        return provider.Retrieve(path).Open(FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    /// <summary>
    /// Opens the file for writing.
    /// If the file already exists, it will be overwritten.
    /// </summary>
    /// <param name="provider">The file provider.</param>
    /// <param name="path">The file path to be opened.</param>
    /// <returns>A <see cref="Stream"/> to the file.</returns>
    public static Stream OpenWrite(this IFileProvider provider, FilePath path)
    {
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentNullException.ThrowIfNull(path);

        return provider.Retrieve(path)
            .Open(FileMode.Create, FileAccess.Write, FileShare.None);
    }

    /// <summary>
    /// Opens a text file, reads all the text in the file, and then closes the file.
    /// </summary>
    /// <param name="provider">The file provider.</param>
    /// <param name="path">The file to read from.</param>
    /// <param name="encoding">The encoding applied to the contents of the file.</param>
    /// <returns>A string containing all text in the file.</returns>
    public static string ReadAllText(this IFileProvider provider, FilePath path, Encoding? encoding = null)
    {
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentNullException.ThrowIfNull(path);

        return provider.Retrieve(path)
            .ReadAllText(encoding);
    }

    /// <summary>
    /// Asynchronously opens a text file, reads all the text in the file, and then closes the file.
    /// </summary>
    /// <param name="provider">The file provider.</param>
    /// <param name="path">The file path.</param>
    /// <param name="cancellationToken">
    /// The token to monitor for cancellation requests.
    /// The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous read operation,
    /// which wraps the string containing all text in the file.
    /// </returns>
    public static async Task<string> ReadAllTextAsync(
        this IFileProvider provider,
        FilePath path,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentNullException.ThrowIfNull(path);

        return await provider.Retrieve(path)
            .ReadAllTextAsync(cancellationToken);
    }

    /// <summary>
    /// Asynchronously opens a text file, reads all the text in the file, and then closes the file.
    /// </summary>
    /// <param name="provider">The file provider.</param>
    /// <param name="path">The file path.</param>
    /// <param name="encoding">The encoding applied to the contents of the file.</param>
    /// <param name="cancellationToken">
    /// The token to monitor for cancellation requests.
    /// The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous read operation,
    /// which wraps the string containing all text in the file.
    /// </returns>
    public static async Task<string> ReadAllTextAsync(
        this IFileProvider provider,
        FilePath path,
        Encoding encoding,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentNullException.ThrowIfNull(path);

        return await provider.Retrieve(path)
            .ReadAllTextAsync(encoding, cancellationToken);
    }

    /// <summary>
    /// Creates a new file, writes the specified string to the file, and then closes the file.
    /// If the target file already exists, it is overwritten.
    /// </summary>
    /// <param name="provider">The file provider.</param>
    /// <param name="path">The file to write to.</param>
    /// <param name="contents">The string to write to the file.</param>
    /// <param name="encoding">The encoding to apply to the string.</param>
    public static void WriteAllText(
        this IFileProvider provider,
        FilePath path,
        string contents,
        Encoding? encoding = null)
    {
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentNullException.ThrowIfNull(path);

        provider.Retrieve(path)
            .WriteAllText(contents, encoding);
    }

    /// <summary>
    /// Creates a new file, writes the specified string to the file, and then closes the file.
    /// If the target file already exists, it is overwritten.
    /// </summary>
    /// <param name="provider">The file provider.</param>
    /// <param name="path">The file to write to.</param>
    /// <param name="contents">The string to write to the file.</param>
    /// <param name="cancellationToken">
    /// The token to monitor for cancellation requests.
    /// The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    public static async Task WriteAllTextAsync(
        this IFileProvider provider,
        FilePath path,
        string contents,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentNullException.ThrowIfNull(path);

        await provider.Retrieve(path)
            .WriteAllTextAsync(contents, cancellationToken);
    }

    /// <summary>
    /// Asynchronously creates a new file, writes the specified string to the file using the
    /// specified encoding, and then closes the file. If the target file already exists,
    /// it is truncated and overwritten.
    /// </summary>
    /// <param name="provider">The file provider.</param>
    /// <param name="path">The file to write to.</param>
    /// <param name="contents">The string to write to the file.</param>
    /// <param name="encoding">The encoding to apply to the string.</param>
    /// <param name="cancellationToken">
    /// The token to monitor for cancellation requests.
    /// The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    public static async Task WriteAllTextAsync(
        this IFileProvider provider,
        FilePath path,
        string contents,
        Encoding encoding,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentNullException.ThrowIfNull(path);

        await provider.Retrieve(path)
            .WriteAllTextAsync(contents, encoding, cancellationToken);
    }
}
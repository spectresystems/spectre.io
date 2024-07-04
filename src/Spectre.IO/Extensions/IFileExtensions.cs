using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Spectre.IO;

/// <summary>
/// Contains extension methods for <see cref="IFile"/>.
/// </summary>
public static class IFileExtensions
{
    /// <summary>
    /// Opens the file using the specified options.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <param name="mode">The mode.</param>
    /// <returns>A <see cref="Stream"/> to the file.</returns>
    public static Stream Open(this IFile file, FileMode mode)
    {
        ArgumentNullException.ThrowIfNull(file);

        return file.Open(
            mode,
            mode == FileMode.Append ? FileAccess.Write : FileAccess.ReadWrite,
            FileShare.None);
    }

    /// <summary>
    /// Opens the file using the specified options.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <param name="mode">The mode.</param>
    /// <param name="access">The access.</param>
    /// <returns>A <see cref="Stream"/> to the file.</returns>
    public static Stream Open(this IFile file, FileMode mode, FileAccess access)
    {
        ArgumentNullException.ThrowIfNull(file);

        return file.Open(mode, access, FileShare.None);
    }

    /// <summary>
    /// Opens the file for reading.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <returns>A <see cref="Stream"/> to the file.</returns>
    public static Stream OpenRead(this IFile file)
    {
        ArgumentNullException.ThrowIfNull(file);

        return file.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    /// <summary>
    /// Opens the file for writing.
    /// If the file already exists, it will be overwritten.
    /// </summary>
    /// <param name="file">The file to be opened.</param>
    /// <returns>A <see cref="Stream"/> to the file.</returns>
    public static Stream OpenWrite(this IFile file)
    {
        ArgumentNullException.ThrowIfNull(file);

        return file.Open(FileMode.Create, FileAccess.Write, FileShare.None);
    }

    /// <summary>
    /// Tries copying the file to the specified destination path.
    /// </summary>
    /// <param name="file">The file to move.</param>
    /// <param name="destination">The destination path.</param>
    /// <param name="overwrite">Will overwrite existing destination file if set to <c>true</c>.</param>
    /// <param name="result">The result if the operation succeeded, otherwise <c>null</c>.</param>
    /// <returns>Whether or not the operation succeeded.</returns>
    public static bool TryCopy(this IFile file, FilePath destination, bool overwrite, out IFile? result)
    {
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullException.ThrowIfNull(destination);

        try
        {
            result = file.Copy(destination, overwrite);
            return result.Exists;
        }
        catch
        {
            result = null;
            return false;
        }
    }

    /// <summary>
    /// Tries moving the file to the specified destination path.
    /// </summary>
    /// <param name="file">The file to move.</param>
    /// <param name="destination">The destination path.</param>
    /// <param name="result">The result if the operation succeeded, otherwise <c>null</c>.</param>
    /// <returns>Whether or not the operation succeeded.</returns>
    public static bool TryMove(this IFile file, FilePath destination, [NotNullWhen(true)] out IFile? result)
    {
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullException.ThrowIfNull(destination);

        return TryMove(file, destination, false, out result);
    }

    /// <summary>
    /// Tries moving the file to the specified destination path.
    /// </summary>
    /// <param name="file">The file to move.</param>
    /// <param name="destination">The destination path.</param>
    /// <param name="overwrite">Will overwrite existing destination file if set to <c>true</c>.</param>
    /// <param name="result">The result if the operation succeeded, otherwise <c>null</c>.</param>
    /// <returns>Whether or not the operation succeeded.</returns>
    public static bool TryMove(
        this IFile file,
        FilePath destination,
        bool overwrite,
        [NotNullWhen(true)] out IFile? result)
    {
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullException.ThrowIfNull(destination);

        try
        {
            result = file.Move(destination, overwrite);
            return result.Exists;
        }
        catch
        {
            result = null;
            return false;
        }
    }

    /// <summary>
    /// Tries deleting the file to the specified destination path.
    /// </summary>
    /// <param name="file">The file to delete.</param>
    /// <returns>Whether or not the operation succeeded.</returns>
    public static bool TryDelete(this IFile file)
    {
        ArgumentNullException.ThrowIfNull(file);

        try
        {
            file.Delete();
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Opens a text file, reads all the text in the file, and then closes the file.
    /// </summary>
    /// <param name="file">The file to read from.</param>
    /// <param name="encoding">The encoding applied to the contents of the file.</param>
    /// <returns>A string containing all text in the file.</returns>
    public static string ReadAllText(
        this IFile file,
        Encoding? encoding = null)
    {
        ArgumentNullException.ThrowIfNull(file);

        using (var reader = new StreamReader(file.OpenRead(), encoding, true, -1, false))
        {
            return reader.ReadToEnd();
        }
    }

    /// <summary>
    /// Asynchronously opens a text file, reads all the text in the file, and then closes the file.
    /// </summary>
    /// <param name="file">The file to read from.</param>
    /// <param name="cancellationToken">
    /// The token to monitor for cancellation requests.
    /// The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous read operation,
    /// which wraps the string containing all text in the file.
    /// </returns>
    public static async Task<string> ReadAllTextAsync(this IFile file, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);

        using (var reader = new StreamReader(file.OpenRead()))
        {
            return await reader.ReadToEndAsync();
        }
    }

    /// <summary>
    /// Asynchronously opens a text file, reads all the text in the file, and then closes the file.
    /// </summary>
    /// <param name="file">The file to read from.</param>
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
        this IFile file,
        Encoding encoding,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);

        using (var reader = new StreamReader(file.OpenRead(), encoding, true, -1, false))
        {
            return await reader.ReadToEndAsync();
        }
    }

    /// <summary>
    /// Creates a new file, writes the specified string to the file, and then closes the file.
    /// If the target file already exists, it is overwritten.
    /// </summary>
    /// <param name="file">The file to write to.</param>
    /// <param name="contents">The string to write to the file.</param>
    /// <param name="encoding">The encoding to apply to the string.</param>
    public static void WriteAllText(
        this IFile file,
        string contents,
        Encoding? encoding = null)
    {
        ArgumentNullException.ThrowIfNull(file);

        using (var writer = new StreamWriter(file.OpenWrite(), encoding, -1, false))
        {
            writer.Write(contents);
        }
    }

    /// <summary>
    /// Creates a new file, writes the specified string to the file, and then closes the file.
    /// If the target file already exists, it is overwritten.
    /// </summary>
    /// <param name="file">The file to write to.</param>
    /// <param name="contents">The string to write to the file.</param>
    /// <param name="cancellationToken">
    /// The token to monitor for cancellation requests.
    /// The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    public static async Task WriteAllTextAsync(
        this IFile file,
        string contents,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);

        using (var writer = new StreamWriter(file.OpenWrite()))
        {
            await writer.WriteAsync(contents);
        }
    }

    /// <summary>
    /// Creates a new file, writes the specified string to the file, and then closes the file.
    /// If the target file already exists, it is overwritten.
    /// </summary>
    /// <param name="file">The file to write to.</param>
    /// <param name="contents">The string to write to the file.</param>
    /// <param name="encoding">The encoding to apply to the string.</param>
    /// <param name="cancellationToken">
    /// The token to monitor for cancellation requests.
    /// The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    public static async Task WriteAllTextAsync(
        this IFile file,
        string contents,
        Encoding encoding,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);

        using (var writer = new StreamWriter(file.OpenWrite(), encoding, -1, false))
        {
            await writer.WriteAsync(contents);
        }
    }
}
using System.Text;

namespace Spectre.IO.Testing;

/// <summary>
/// Contains extensions for <see cref="FakeFile"/>.
/// </summary>
[PublicAPI]
public static class FakeFileExtensions
{
    /// <summary>
    /// Sets the last write time of the provided file.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <param name="lastWriteTime">The last write time.</param>
    /// <returns>The same <see cref="FakeFile"/> instance so that multiple calls can be chained.</returns>
    public static FakeFile SetLastWriteTime(this FakeFile file, DateTime lastWriteTime)
    {
        ArgumentNullException.ThrowIfNull(file);

        file.LastWriteTime = lastWriteTime;
        return file;
    }

    /// <summary>
    /// Sets the content of the provided file.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <param name="content">The content.</param>
    /// <param name="encoding">The text encoding to use, or <c>null</c> to use the default encoding.</param>
    /// <returns>The same <see cref="FakeFile"/> instance so that multiple calls can be chained.</returns>
    public static FakeFile SetTextContent(this FakeFile file, string content, Encoding? encoding = null)
    {
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullException.ThrowIfNull(content);

        encoding ??= Encoding.Default;

        using (var stream = file.Open(FileMode.Create, FileAccess.Write, FileShare.None))
        {
            var bytes = encoding.GetBytes(content);
            stream.Write(bytes, 0, bytes.Length);
            return file;
        }
    }

    /// <summary>
    /// Gets the text content of the file.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <param name="encoding">The text encoding to use, or <c>null</c> to use the default encoding.</param>
    /// <returns>The text content of the file.</returns>
    [Obsolete("Use `IFile.ReadAllText(...)` instead")]
    public static string GetTextContent(this FakeFile file, Encoding? encoding = null)
    {
        ArgumentNullException.ThrowIfNull(file);

        if (!file.Exists)
        {
            throw new FileNotFoundException("File could not be found.", file.Path.FullPath);
        }

        encoding ??= Encoding.Default;

        using (var stream = file.OpenRead())
        using (var reader = new StreamReader(stream, encoding))
        {
            return reader.ReadToEnd();
        }
    }

    /// <summary>
    /// Hides the specified file.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <returns>The same <see cref="FakeFile"/> instance so that multiple calls can be chained.</returns>
    public static FakeFile Hide(this FakeFile file)
    {
        if (file == null)
        {
            throw new ArgumentNullException(nameof(file));
        }

        file.Hidden = true;
        return file;
    }
}
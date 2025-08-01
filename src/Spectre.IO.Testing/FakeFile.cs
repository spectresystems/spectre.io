using System.Diagnostics;

namespace Spectre.IO.Testing;

/// <summary>
/// Represents a fake file.
/// </summary>
[PublicAPI]
[DebuggerDisplay("{Path,nq}")]
public sealed class FakeFile : IFile
{
    private readonly FakeFileSystemTree _tree;

    /// <summary>
    /// Gets the path to the file.
    /// </summary>
    /// <value>The path.</value>
    public FilePath Path { get; }

    /// <summary>
    /// Gets the source path for a symbolic link.
    /// </summary>
    public FakeFile? SymbolicLink { get; internal init; }

    /// <inheritdoc/>
    Path IFileSystemInfo.Path => Path;

    /// <inheritdoc/>
    public bool Exists { get; internal set; }

    /// <inheritdoc/>
    public bool Hidden { get; internal set; }

    /// <inheritdoc/>
    public long Length { get; private set; }

    /// <inheritdoc/>
    public DateTime LastWriteTime { get; set; }

    /// <inheritdoc/>
    public FileAttributes Attributes { get; set; }

    /// <summary>
    /// Gets the length of the content.
    /// </summary>
    /// <value>
    /// The length of the content.
    /// </value>
    public long ContentLength
    {
        get { return Length; }
        internal set { Length = value; }
    }

    /// <summary>
    /// Gets the content.
    /// </summary>
    /// <value>The content.</value>
    public byte[] Content { get; internal set; }

    internal object ContentLock { get; } = new object();

    internal FakeFile(FakeFileSystemTree tree, FilePath path)
    {
        _tree = tree;

        Path = path;
        Exists = false;
        Hidden = false;
        Content = new byte[4096];
    }

    /// <inheritdoc/>
    public IFile Copy(FilePath destination, bool overwrite)
    {
        ArgumentNullException.ThrowIfNull(destination);

        _tree.CopyFile(this, destination, overwrite);
        return _tree.FindFile(destination) ?? new FakeFile(_tree, destination);
    }

    /// <inheritdoc/>
    public void CreateSymbolicLink(FilePath destination)
    {
        ArgumentNullException.ThrowIfNull(destination);

        _tree.CreateSymbolicLink(this, destination);
    }

    /// <inheritdoc/>
    public IFile Move(FilePath destination, bool overwrite)
    {
        _tree.MoveFile(this, destination, overwrite);
        return _tree.FindFile(destination) ?? new FakeFile(_tree, destination);
    }

    /// <inheritdoc/>
    public Stream Open(FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
    {
        if (SymbolicLink != null)
        {
            return SymbolicLink.Open(fileMode, fileAccess, fileShare);
        }

        var position = GetPosition(fileMode, out var fileWasCreated);
        if (fileWasCreated)
        {
            _tree.CreateFile(this);
        }

        return new FakeFileStream(this) { Position = position };
    }

    /// <inheritdoc/>
    public void Delete()
    {
        _tree.DeleteFile(this);
    }

    /// <summary>
    /// Resizes the file.
    /// </summary>
    /// <param name="offset">The offset.</param>
    public void Resize(long offset)
    {
        if (SymbolicLink != null)
        {
            SymbolicLink.Resize(offset);
            return;
        }

        if (Length < offset)
        {
            Length = offset;
        }

        if (Content.Length >= Length)
        {
            return;
        }

        var buffer = new byte[Length * 2];
        Buffer.BlockCopy(Content, 0, buffer, 0, Content.Length);
        Content = buffer;
    }

    /// <inheritdoc/>
    public void Refresh()
    {
    }

    private long GetPosition(FileMode fileMode, out bool fileWasCreated)
    {
        fileWasCreated = false;

        if (Exists)
        {
            switch (fileMode)
            {
                case FileMode.CreateNew:
                    throw new IOException();
                case FileMode.Create:
                case FileMode.Truncate:
                    Length = 0;
                    return 0;
                case FileMode.Open:
                case FileMode.OpenOrCreate:
                    return 0;
                case FileMode.Append:
                    return Length;
            }
        }
        else
        {
            switch (fileMode)
            {
                case FileMode.Create:
                case FileMode.Append:
                case FileMode.CreateNew:
                case FileMode.OpenOrCreate:
                    fileWasCreated = true;
                    Exists = true;
                    return Length;
                case FileMode.Open:
                case FileMode.Truncate:
                    throw new FileNotFoundException();
            }
        }

        throw new NotSupportedException();
    }
}
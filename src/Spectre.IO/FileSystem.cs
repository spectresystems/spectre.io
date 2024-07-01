using System;

namespace Spectre.IO;

/// <summary>
/// A physical file system implementation.
/// </summary>
public sealed class FileSystem : IFileSystem
{
    /// <summary>
    /// Gets the default <see cref="FileSystem"/> instance.
    /// </summary>
    public static FileSystem Shared { get; } = new();

    /// <inheritdoc/>
    public IPathComparer Comparer { get; }

    /// <inheritdoc/>
    public IFileProvider File { get; }

    /// <inheritdoc/>
    public IDirectoryProvider Directory { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileSystem"/> class.
    /// </summary>
    /// <param name="comparer">The path comparer to use.</param>
    public FileSystem(IPathComparer? comparer = null)
    {
        Comparer = comparer ?? PathComparer.Default;
        File = new FileProvider();
        Directory = new DirectoryProvider();
    }
}
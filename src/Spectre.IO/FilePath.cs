using Spectre.IO.Internal;

namespace Spectre.IO;

/// <summary>
/// Represents a file path.
/// </summary>
[PublicAPI]
public sealed class FilePath : Path, IEquatable<FilePath>, IComparable<FilePath>
{
    /// <summary>
    /// Gets a value indicating whether this path has a file extension.
    /// </summary>
    /// <value>
    /// <c>true</c> if this file path has a file extension; otherwise, <c>false</c>.
    /// </value>
    public bool HasExtension => PathHelper.HasExtension(this);

    /// <summary>
    /// Initializes a new instance of the <see cref="FilePath"/> class.
    /// </summary>
    /// <param name="path">The path.</param>
    public FilePath(string path)
        : base(path)
    {
    }

    /// <summary>
    /// Gets the directory part of the path.
    /// </summary>
    /// <returns>The directory part of the path.</returns>
    public DirectoryPath GetDirectory()
    {
        var directory = PathHelper.GetDirectoryName(this);
        if (string.IsNullOrWhiteSpace(directory))
        {
            directory = "./";
        }

        return new DirectoryPath(directory);
    }

    /// <summary>
    /// Gets the filename.
    /// </summary>
    /// <returns>The filename.</returns>
    public FilePath GetFilename()
    {
        var filename = PathHelper.GetFileName(this) ?? "./";
        return new FilePath(filename);
    }

    /// <summary>
    /// Gets the filename without its extension.
    /// </summary>
    /// <returns>The filename without its extension.</returns>
    public FilePath GetFilenameWithoutExtension()
    {
        var filename = PathHelper.GetFileNameWithoutExtension(this);
        return filename == null
            ? new FilePath("./")
            : new FilePath(filename);
    }

    /// <summary>
    /// Gets the file extension.
    /// </summary>
    /// <returns>The file extension.</returns>
    public string? GetExtension()
    {
        var filename = PathHelper.GetFileName(this);
        if (filename == null)
        {
            return null;
        }

        var index = filename.LastIndexOf('.');
        return index != -1
            ? filename.Substring(index, filename.Length - index)
            : null;
    }

    /// <summary>
    /// Changes the file extension of the path.
    /// </summary>
    /// <param name="extension">The new extension.</param>
    /// <returns>A new <see cref="FilePath"/> with a new extension.</returns>
    public FilePath ChangeExtension(string extension)
    {
        var filename = PathHelper.ChangeExtension(this, extension);
        return filename == null
            ? new FilePath("./")
            : new FilePath(filename);
    }

    /// <summary>
    /// Appends a file extension to the path.
    /// </summary>
    /// <param name="extension">The extension.</param>
    /// <returns>A new <see cref="FilePath"/> with an appended extension.</returns>
    public FilePath AppendExtension(string extension)
    {
        ArgumentNullException.ThrowIfNull(extension);

        if (!extension.StartsWith(".", StringComparison.OrdinalIgnoreCase))
        {
            extension = $".{extension}";
        }

        return new FilePath(string.Concat(FullPath, extension));
    }

    /// <summary>
    /// Removes the file extension from the path.
    /// </summary>
    /// <returns>A new <see cref="FilePath"/> with the extension removed.</returns>
    public FilePath RemoveExtension()
    {
        var filename = PathHelper.GetFileNameWithoutExtension(this);
        return filename == null
            ? new FilePath(FullPath)
            : GetDirectory().CombineWithFilePath(new FilePath(filename));
    }

    /// <summary>
    /// Makes the path absolute (if relative) using the specified directory path.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>An absolute path.</returns>
    public FilePath MakeAbsolute(DirectoryPath path)
    {
        ArgumentNullException.ThrowIfNull(path);

        if (path.IsRelative)
        {
            throw new InvalidOperationException("Cannot make a file path absolute with a relative directory path.");
        }

        return IsRelative
            ? path.CombineWithFilePath(this).Collapse()
            : new FilePath(FullPath).Collapse();
    }

    /// <summary>
    /// Makes the path absolute (if relative) using the current working directory.
    /// </summary>
    /// <param name="environment">The environment.</param>
    /// <returns>An absolute path.</returns>
    public FilePath MakeAbsolute(IEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(environment);

        // First expand the directory (convert ~ into correct path)
        var result = Expand(environment);

        // Combine it with the working directory if relative
        result = result.IsRelative ? environment.WorkingDirectory.CombineWithFilePath(result) : result;

        // Collapse the path
        return result.Collapse();
    }

    /// <summary>
    /// Collapses a <see cref="FilePath"/> containing ellipses.
    /// </summary>
    /// <returns>A collapsed <see cref="FilePath"/>.</returns>
    public FilePath Collapse()
    {
        return new FilePath(PathCollapser.Collapse(this));
    }

    /// <summary>
    /// Expands a <see cref="FilePath"/> containing placeholders
    /// such as <c>~</c>.
    /// </summary>
    /// <param name="environment">The environment.</param>
    /// <returns>An expanded <see cref="FilePath"/>.</returns>
    public FilePath Expand(IEnvironment environment)
    {
        return new FilePath(PathExpander.Expand(this, environment));
    }

    /// <summary>
    /// Performs an implicit conversion from <see cref="string"/> to <see cref="FilePath"/>.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>A <see cref="FilePath"/>.</returns>
    public static implicit operator FilePath(string path)
    {
        return FromString(path);
    }

    /// <summary>
    /// Performs a conversion from <see cref="string"/> to <see cref="FilePath"/>.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>A <see cref="FilePath"/>.</returns>
    public static FilePath FromString(string path)
    {
        return new FilePath(path);
    }

    /// <summary>
    /// Get the relative path to another directory.
    /// </summary>
    /// <param name="to">The target directory path.</param>
    /// <returns>A <see cref="DirectoryPath"/>.</returns>
    public DirectoryPath GetRelativePath(DirectoryPath to)
    {
        return GetDirectory().GetRelativePath(to);
    }

    /// <summary>
    /// Get the relative path to another file.
    /// </summary>
    /// <param name="to">The target file path.</param>
    /// <returns>A <see cref="FilePath"/>.</returns>
    public FilePath GetRelativePath(FilePath to)
    {
        return GetDirectory().GetRelativePath(to);
    }

    /// <inheritdoc />
    public int CompareTo(FilePath? other)
    {
        return PathComparer.Default.Compare(this, other);
    }

    /// <inheritdoc />
    public bool Equals(FilePath? other)
    {
        return PathComparer.Default.Equals(this, other);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj)
               || (obj is FilePath other && Equals(other));
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return PathComparer.Default.GetHashCode(this);
    }
}
using System;
using System.IO;
using System.Linq;
using Spectre.IO.Internal;

namespace Spectre.IO;

/// <summary>
/// Represents a directory path.
/// </summary>
public sealed class DirectoryPath : Path, IEquatable<DirectoryPath>, IComparable<DirectoryPath>
{
    /// <summary>
    /// Gets a value indicating whether or not the current
    /// path is considered to be a root.
    /// </summary>
    public bool IsRoot { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DirectoryPath"/> class.
    /// </summary>
    /// <param name="path">The path.</param>
    public DirectoryPath(string path)
        : base(path)
    {
        IsRoot = PathHelper.IsPathRooted(FullPath)
                 && ((IsUNC && Segments.Count == 2)
                     || (Segments.Count == 1));
    }

    /// <summary>
    /// Gets the name of the directory.
    /// </summary>
    /// <returns>The directory name.</returns>
    /// <remarks>
    ///    If this is passed a file path, it will return the file name.
    ///    This is by-and-large equivalent to how DirectoryInfo handles this scenario.
    ///    If we wanted to return the *actual* directory name, we'd need to pull in IFileSystem,
    ///    and do various checks to make sure things exists.
    /// </remarks>
    public string GetDirectoryName()
    {
        if (Segments.Count == 0)
        {
            return string.Empty;
        }

        return Segments[Segments.Count - 1];
    }

    /// <summary>
    /// Combines the current path with the file name of a <see cref="FilePath"/>.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>A combination of the current path and the file name of the provided <see cref="FilePath"/>.</returns>
    public FilePath GetFilePath(FilePath path)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        return new FilePath(PathHelper.Combine(FullPath, path.GetFilename().FullPath));
    }

    /// <summary>
    /// Gets the parent directory.
    /// </summary>
    /// <returns>The parent directory, or <c>null</c> if none.</returns>
    public DirectoryPath? GetParent()
    {
        if (IsRoot)
        {
            return null;
        }

        var seg = Segments.Take(Segments.Count - 1);
        if (IsUNC)
        {
            // Skip the root
            seg = seg.Skip(1);
        }

        var path = string.Join(Separator.ToString(), seg);
        if (IsUNC)
        {
            path = string.Concat("\\\\", path);
        }

        return new DirectoryPath(path);
    }

    /// <summary>
    /// Combines the current path with a <see cref="FilePath"/>.
    /// The provided <see cref="FilePath"/> must be relative.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>A combination of the current path and the provided <see cref="FilePath"/>.</returns>
    public FilePath CombineWithFilePath(FilePath path)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        if (!path.IsRelative)
        {
            throw new InvalidOperationException("Cannot combine a directory path with an absolute file path.");
        }

        return new FilePath(PathHelper.Combine(FullPath, path.FullPath));
    }

    /// <summary>
    /// Combines the current path with another <see cref="DirectoryPath"/>.
    /// The provided <see cref="DirectoryPath"/> must be relative.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>A combination of the current path and the provided <see cref="DirectoryPath"/>.</returns>
    public DirectoryPath Combine(DirectoryPath path)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        if (!path.IsRelative)
        {
            throw new InvalidOperationException("Cannot combine a directory path with an absolute directory path.");
        }

        return new DirectoryPath(PathHelper.Combine(FullPath, path.FullPath));
    }

    /// <summary>
    /// Makes the path absolute to another (absolute) path.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>An absolute path.</returns>
    public DirectoryPath MakeAbsolute(DirectoryPath path)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        if (path.IsRelative)
        {
            throw new IOException("The provided path cannot be relative.");
        }

        return IsRelative
            ? path.Combine(this).Collapse()
            : new DirectoryPath(FullPath).Collapse();
    }

    /// <summary>
    /// Makes the path absolute (if relative) using the current working directory.
    /// </summary>
    /// <param name="environment">The environment.</param>
    /// <returns>An absolute path.</returns>
    public DirectoryPath MakeAbsolute(IEnvironment environment)
    {
        if (environment == null)
        {
            throw new ArgumentNullException(nameof(environment));
        }

        // First expand the directory (convert ~ into correct path)
        var result = Expand(environment);

        // Combine it with the working directory if relative
        result = result.IsRelative ? environment.WorkingDirectory.Combine(result) : result;

        // Collapse the path
        return result.Collapse();
    }

    /// <summary>
    /// Collapses a <see cref="DirectoryPath"/> containing ellipses.
    /// </summary>
    /// <returns>A collapsed <see cref="FilePath"/>.</returns>
    public DirectoryPath Collapse()
    {
        return new DirectoryPath(PathCollapser.Collapse(this));
    }

    /// <summary>
    /// Expands a <see cref="DirectoryPath"/> containing placeholders
    /// such as <c>~</c>.
    /// </summary>
    /// <param name="environment">The environment.</param>
    /// <returns>An expanded <see cref="FilePath"/>.</returns>
    public DirectoryPath Expand(IEnvironment environment)
    {
        return new DirectoryPath(PathExpander.Expand(this, environment));
    }

    /// <summary>
    /// Performs an implicit conversion from <see cref="string"/> to <see cref="DirectoryPath"/>.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>A <see cref="DirectoryPath"/>.</returns>
    public static implicit operator DirectoryPath(string path)
    {
        return FromString(path);
    }

    /// <summary>
    /// Performs a conversion from <see cref="string"/> to <see cref="DirectoryPath"/>.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>A <see cref="DirectoryPath"/>.</returns>
    public static DirectoryPath FromString(string path)
    {
        return new DirectoryPath(path);
    }

    /// <summary>
    /// Get the relative path to another directory.
    /// </summary>
    /// <param name="to">The target directory path.</param>
    /// <returns>A <see cref="DirectoryPath"/>.</returns>
    public DirectoryPath GetRelativePath(DirectoryPath to)
    {
        return RelativePathResolver.Resolve(this, to);
    }

    /// <summary>
    /// Get the relative path to another file.
    /// </summary>
    /// <param name="to">The target file path.</param>
    /// <returns>A <see cref="FilePath"/>.</returns>
    public FilePath GetRelativePath(FilePath to)
    {
        if (to == null)
        {
            throw new ArgumentNullException(nameof(to));
        }

        return GetRelativePath(to.GetDirectory()).GetFilePath(to.GetFilename());
    }

    /// <inheritdoc />
    public int CompareTo(DirectoryPath? other)
    {
        return PathComparer.Default.Compare(this, other);
    }

    /// <inheritdoc />
    public bool Equals(DirectoryPath? other)
    {
        return PathComparer.Default.Equals(this, other);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj)
               || (obj is DirectoryPath other && Equals(other));
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return PathComparer.Default.GetHashCode(this);
    }
}
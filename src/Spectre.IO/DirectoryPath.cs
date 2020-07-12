using System;
using System.IO;
using Spectre.IO.Internal;

namespace Spectre.IO
{
    /// <summary>
    /// Represents a directory path.
    /// </summary>
    public sealed class DirectoryPath : Path
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryPath"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        public DirectoryPath(string path)
            : base(path)
        {
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
                : new DirectoryPath(FullPath);
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

            return IsRelative
                ? environment.WorkingDirectory.Combine(this).Collapse()
                : new DirectoryPath(FullPath);
        }

        /// <summary>
        /// Collapses a <see cref="DirectoryPath"/> containing ellipses.
        /// </summary>
        /// <returns>A collapsed <see cref="DirectoryPath"/>.</returns>
        public DirectoryPath Collapse()
        {
            return new DirectoryPath(PathCollapser.Collapse(this));
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
    }
}
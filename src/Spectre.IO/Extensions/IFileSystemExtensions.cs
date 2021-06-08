using System;

namespace Spectre.IO
{
    /// <summary>
    /// Contains extensions for <see cref="IFileSystem"/>.
    /// </summary>
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

            var file = fileSystem.File.Retrieve(path);
            return file?.Exists == true;
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

            var directory = fileSystem.Directory.Retrieve(path);
            return directory?.Exists == true;
        }
    }
}
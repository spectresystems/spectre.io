using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Spectre.IO
{
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
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

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
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            return file.Open(mode, access, FileShare.None);
        }

        /// <summary>
        /// Opens the file for reading.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>A <see cref="Stream"/> to the file.</returns>
        public static Stream OpenRead(this IFile file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

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
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

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
        public static bool TryMove(this IFile file, FilePath destination, bool overwrite, [NotNullWhen(true)] out IFile? result)
        {
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
    }
}
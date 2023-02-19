using System;

namespace Spectre.IO.Testing
{
    /// <summary>
    /// Contains extensions for <see cref="FakeDirectory"/>.
    /// </summary>
    public static class FakeDirectoryExtensions
    {
        /// <summary>
        /// Sets the last write time of the provided file.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="lastWriteTime">The last write time.</param>
        /// <returns>The same <see cref="FakeDirectory"/> instance so that multiple calls can be chained.</returns>
        public static FakeDirectory SetLastWriteTime(this FakeDirectory directory, DateTime lastWriteTime)
        {
            if (directory == null)
            {
                throw new ArgumentNullException(nameof(directory));
            }

            directory.LastWriteTime = lastWriteTime;
            return directory;
        }


        /// <summary>
        /// Hides the specified directory.
        /// </summary>
        /// <param name="directory">The directory to hide.</param>
        /// <returns>The same <see cref="FakeDirectory"/> instance so that multiple calls can be chained.</returns>
        public static FakeDirectory Hide(this FakeDirectory directory)
        {
            if (directory == null)
            {
                throw new ArgumentNullException(nameof(directory));
            }

            directory.Hidden = true;
            return directory;
        }
    }
}
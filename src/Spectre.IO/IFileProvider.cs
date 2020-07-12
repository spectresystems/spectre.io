using System.Diagnostics.CodeAnalysis;

namespace Spectre.IO
{
    /// <summary>
    /// Represents a file provider.
    /// </summary>
    public interface IFileProvider
    {
        /// <summary>
        /// Gets a <see cref="IFile"/> instance.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <returns>An <see cref="IFile"/> instance.</returns>
        [SuppressMessage("Naming", "CA1716:Identifiers should not match keywords")]
        IFile Retrieve(FilePath path);
    }
}
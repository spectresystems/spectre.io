using System.Diagnostics.CodeAnalysis;

namespace Spectre.IO
{
    /// <summary>
    /// Represents a directory provider.
    /// </summary>
    public interface IDirectoryProvider
    {
        /// <summary>
        /// Gets a <see cref="IDirectory"/> instance.
        /// </summary>
        /// <param name="path">The directory path.</param>
        /// <returns>An <see cref="IDirectory"/> instance.</returns>
        [SuppressMessage("Naming", "CA1716:Identifiers should not match keywords")]
        IDirectory Retrieve(DirectoryPath path);
    }
}
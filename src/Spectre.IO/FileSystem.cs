namespace Spectre.IO
{
    /// <summary>
    /// A physical file system implementation.
    /// </summary>
    public sealed class FileSystem : IFileSystem
    {
        /// <inheritdoc/>
        public IFileProvider File { get; }

        /// <inheritdoc/>
        public IDirectoryProvider Directory { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystem"/> class.
        /// </summary>
        public FileSystem()
        {
            File = new FileProvider();
            Directory = new DirectoryProvider();
        }
    }
}
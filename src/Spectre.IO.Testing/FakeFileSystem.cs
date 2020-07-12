using System;
using System.IO;

namespace Spectre.IO.Testing
{
    /// <summary>
    /// Represents a fake file system.
    /// </summary>
    public sealed class FakeFileSystem : IFileSystem
    {
        private readonly FakeFileProvider _fileProvider;
        private readonly FakeDirectoryProvider _directoryProvider;
        private readonly IEnvironment _environment;

        /// <inheritdoc/>
        public IFileProvider File => _fileProvider;

        /// <inheritdoc/>
        public IDirectoryProvider Directory => _directoryProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeFileSystem"/> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        public FakeFileSystem(IEnvironment environment)
        {
            var tree = new FakeFileSystemTree(environment);

            _fileProvider = new FakeFileProvider(tree);
            _directoryProvider = new FakeDirectoryProvider(tree);
            _environment = environment;
        }

        /// <summary>
        /// Gets a <see cref="FakeFile"/> instance representing the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>A <see cref="FakeFile"/> instance representing the specified path.</returns>
        public FakeFile GetFakeFile(FilePath path)
        {
            return _fileProvider.Get(path);
        }

        /// <summary>
        /// Gets a <see cref="FakeDirectory" /> instance representing the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>A <see cref="FakeDirectory" /> instance representing the specified path.</returns>
        public FakeDirectory GetFakeDirectory(DirectoryPath path)
        {
            return _directoryProvider.Get(path);
        }

        /// <summary>
        /// Ensures that the specified file does not exist.
        /// </summary>
        /// <param name="path">The path.</param>
        public void EnsureFileDoesNotExist(FilePath path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            var file = GetFakeFile(path);
            if (file?.Exists == true)
            {
                file.Delete();
            }
        }

        /// <summary>
        /// Creates a file at the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="attributes">The file attributes to set.</param>
        /// <returns>The same <see cref="FakeFile"/> instance so that multiple calls can be chained.</returns>
        public FakeFile CreateFile(FilePath path, FileAttributes attributes = 0)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            path = path.MakeAbsolute(_environment);

            CreateDirectory(path.GetDirectory());
            var file = GetFakeFile(path);
            if (!file.Exists)
            {
                file.OpenWrite().Dispose();
            }

            file.Attributes = attributes;
            return file;
        }

        /// <summary>
        /// Creates a file at the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="contentsBytes">The file contents.</param>
        /// <returns>The same <see cref="FakeFile"/> instance so that multiple calls can be chained.</returns>
        public FakeFile CreateFile(FilePath path, byte[] contentsBytes)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (contentsBytes == null)
            {
                throw new ArgumentNullException(nameof(contentsBytes));
            }

            path = path.MakeAbsolute(_environment);

            CreateDirectory(path.GetDirectory());
            var file = GetFakeFile(path);
            if (!file.Exists)
            {
                using (var stream = file.OpenWrite())
                {
                    using (var ms = new MemoryStream(contentsBytes))
                    {
                        ms.CopyTo(stream);
                    }
                }
            }

            return file;
        }

        /// <summary>
        /// Creates a directory at the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The same <see cref="FakeDirectory"/> instance so that multiple calls can be chained.</returns>
        public FakeDirectory CreateDirectory(DirectoryPath path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            path = path.MakeAbsolute(_environment);

            var directory = GetFakeDirectory(path);
            if (!directory.Exists)
            {
                directory.Create();
            }

            return directory;
        }
    }
}
using System;
using System.IO;

namespace Spectre.IO.Internal
{
    internal sealed class File : IFile
    {
        private readonly FileInfo _file;

        public FilePath Path { get; }

        Path IFileSystemInfo.Path => Path;

        public bool Exists => _file.Exists;

        public bool Hidden => (_file.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden;

        public DateTime LastWriteTime => _file.LastWriteTime;

        public long Length => _file.Length;

        public FileAttributes Attributes
        {
            get { return _file.Attributes; }
            set { _file.Attributes = value; }
        }

        public File(FilePath path)
        {
            Path = path;
            _file = new FileInfo(path.FullPath);
        }

        public IFile Copy(FilePath destination, bool overwrite)
        {
            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            var result = _file.CopyTo(destination.FullPath, overwrite);
            return new File(result.FullName);
        }

        public IFile Move(FilePath destination, bool overwrite)
        {
            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            _file.MoveTo(destination.FullPath, overwrite);
            return new File(destination.FullPath);
        }

        public void Delete()
        {
            _file.Delete();
        }

        public void Refresh()
        {
            _file.Refresh();
        }

        public Stream Open(FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
        {
            return _file.Open(fileMode, fileAccess, fileShare);
        }

        public void CreateSymbolicLink(FilePath destination)
        {
            if (destination is null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            if (Path.IsRelative)
            {
                throw new InvalidOperationException("Source path cannot be relative");
            }

            if (destination.IsRelative)
            {
                throw new InvalidOperationException("Detination path cannot be relative");
            }

            System.IO.File.CreateSymbolicLink(destination.FullPath, Path.FullPath);
        }
    }
}

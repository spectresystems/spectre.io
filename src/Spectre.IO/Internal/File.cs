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

        public DateTime LastWritetime => _file.LastWriteTime;

        public File(FilePath path)
        {
            Path = path;
            _file = new FileInfo(path.FullPath);
        }

        public void Copy(FilePath destination, bool overwrite)
        {
            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            _file.CopyTo(destination.FullPath, overwrite);
        }

        public void Move(FilePath destination)
        {
            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            _file.MoveTo(destination.FullPath);
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
    }
}
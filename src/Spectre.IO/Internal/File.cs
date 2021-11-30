using System;
using System.IO;
#if !NET6_0_OR_GREATER
using System.Runtime.InteropServices;
using Mono.Unix.Native;
#endif

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

#if NET6_0_OR_GREATER
            System.IO.File.CreateSymbolicLink(destination.FullPath, Path.FullPath);
#else
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (!Win32.CreateSymbolicLink(destination.FullPath, Path.FullPath, Win32.SymbolicLink.File))
                {
                    throw new IOException($"Could not create symbolic link from {Path.FullPath} to {destination.FullPath}");
                }
            }
            else
            {
                if (Syscall.symlink(Path.FullPath, destination.FullPath) != 0)
                {
                    throw new IOException($"Could not create symbolic link from {Path.FullPath} to {destination.FullPath}");
                }
            }
#endif
        }
    }
}

using System.Runtime.InteropServices;
using System.Text;

namespace Spectre.IO.Internal;

internal static partial class Native
{
    public static class Unix
    {
        [DllImport("libc", SetLastError = true)]
        public static extern IntPtr getenv([MarshalAs(UnmanagedType.LPStr)] string name);

        public static string? GetFolder(KnownPath folder)
        {
            switch (folder)
            {
                case KnownPath.ProgramFiles:
                case KnownPath.ProgramFilesX86:
                    return "/user/bin";
                case KnownPath.LocalApplicationData:
                case KnownPath.ApplicationData:
                    var value = getenv("HOME");
                    if (value == IntPtr.Zero)
                    {
                        return null;
                    }

                    var size = 0;
                    while (Marshal.ReadByte(value, size) != 0)
                    {
                        size++;
                    }

                    if (size == 0)
                    {
                        var buffer = new byte[size];
                        Marshal.Copy(value, buffer, 0, size);
                        return Encoding.UTF8.GetString(buffer);
                    }

                    return string.Empty;
                default:
                    return null;
            }
        }
    }

    internal static class Windows
    {
        private static readonly Dictionary<KnownPath, int> _lookup = new Dictionary<KnownPath, int>
        {
            {
                KnownPath.ApplicationData, 0x001a
            },
            {
                KnownPath.CommonApplicationData, 0x0023
            },
            {
                KnownPath.LocalApplicationData, 0x001c
            },
            {
                KnownPath.ProgramFiles, 0x0026
            },
            {
                KnownPath.ProgramFilesX86, 0x002a
            },
            {
                KnownPath.Windows, 0x0024
            },
        };

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, BestFitMapping = false)]
        public static extern int SHGetFolderPath(IntPtr hwndOwner, int nFolder, IntPtr hToken, int dwFlags,
            [Out] StringBuilder lpszPath);

        public static string? GetFolder(KnownPath folder)
        {
            if (!_lookup.ContainsKey(folder))
            {
                return null;
            }

            var builder = new StringBuilder(260);
            var result = SHGetFolderPath(IntPtr.Zero, _lookup[folder], IntPtr.Zero, 0, builder);
            if (result < 0)
            {
                if (result == unchecked((int)0x80131539))
                {
                    throw new PlatformNotSupportedException();
                }

                return null;
            }

            return builder.ToString();
        }
    }
}
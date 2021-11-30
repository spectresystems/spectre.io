#if !NET6_0_OR_GREATER
using System.Runtime.InteropServices;

namespace Spectre.IO.Internal
{
    internal static class Win32
    {
        public enum SymbolicLink
        {
            File = 0,
            Directory = 1,
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, SymbolicLink dwFlags);
    }
}
#endif

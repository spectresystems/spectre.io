using System;
using System.Runtime.InteropServices;

namespace Spectre.IO.Internal
{
    internal static class EnvironmentHelper
    {
        private static bool? _isCoreClr;

        public static bool Is64BitOperativeSystem()
        {
            return RuntimeInformation.OSArchitecture == Architecture.X64
                   || RuntimeInformation.OSArchitecture == Architecture.Arm64;
        }

        public static PlatformFamily GetPlatformFamily()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return PlatformFamily.MacOs;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return PlatformFamily.Linux;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return PlatformFamily.Windows;
            }

#if NET5_0_OR_GREATER
            if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            {
                return PlatformFamily.FreeBSD;
            }
#endif

            return PlatformFamily.Unknown;
        }

        public static bool IsCoreClr()
        {
            return _isCoreClr ?? (_isCoreClr = RuntimeInformation.FrameworkDescription.StartsWith(".NET Core", StringComparison.OrdinalIgnoreCase)).Value;
        }

        public static bool IsUnix()
        {
            return IsUnix(GetPlatformFamily());
        }

        public static bool IsUnix(PlatformFamily family)
        {
            return family == PlatformFamily.Linux
                   || family == PlatformFamily.MacOs;
        }
    }
}
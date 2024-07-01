using System.Runtime.InteropServices;

namespace Spectre.IO.Internal;

internal static class EnvironmentHelper
{
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

    public static PlatformArchitecture GetPlatformArchitecture()
    {
        return RuntimeInformation.OSArchitecture switch
        {
            Architecture.X86 => PlatformArchitecture.X86,
            Architecture.X64 => PlatformArchitecture.X64,
            Architecture.Arm => PlatformArchitecture.Arm,
            Architecture.Arm64 => PlatformArchitecture.Arm64,
#if NET5_0_OR_GREATER
            Architecture.Wasm => PlatformArchitecture.Wasm,
#endif
            _ => PlatformArchitecture.Unknown,
        };
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
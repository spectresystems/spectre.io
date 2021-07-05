using System;
using System.Runtime.InteropServices;

namespace Spectre.IO.Internal
{
    internal static class EnvironmentHelper
    {
        public static PlatformFamily GetPlatformFamily()
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    return PlatformFamily.MacOs;
                }
            }
            catch (PlatformNotSupportedException)
            {
            }

            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    return PlatformFamily.Linux;
                }
            }
            catch (PlatformNotSupportedException)
            {
            }

            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return PlatformFamily.Windows;
                }
            }
            catch (PlatformNotSupportedException)
            {
            }

            return PlatformFamily.Unknown;
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
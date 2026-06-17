using System.Globalization;
using Spectre.IO.Internal;

namespace Spectre.IO;

public enum KnownPath
{
    /// <summary>
    /// The directory that serves as a common repository for application-specific
    /// data for the current roaming user.
    /// </summary>
    ApplicationData,

    /// <summary>
    /// The directory that serves as a common repository for application-specific
    /// data that is used by all users.
    /// </summary>
    CommonApplicationData,

    /// <summary>
    /// The directory that serves as a common repository for application-specific
    /// data that is used by the current, non-roaming user.
    /// </summary>
    LocalApplicationData,

    /// <summary>
    /// The Program Files folder.
    /// </summary>
    ProgramFiles,

    /// <summary>
    /// The Program Files (X86) folder.
    /// </summary>
    ProgramFilesX86,

    /// <summary>
    /// The Windows folder.
    /// </summary>
    Windows,

    /// <summary>
    /// The current user's temporary folder.
    /// </summary>
    LocalTemp,

    /// <summary>
    /// The user's profile folder.
    /// </summary>
    UserProfile,
}

internal static class KnownPathUtilities
{
    public static DirectoryPath GetFolderPath(IPlatform platform, KnownPath path)
    {
        if (path == KnownPath.LocalTemp)
        {
            return new DirectoryPath(System.IO.Path.GetTempPath());
        }

        if (path == KnownPath.UserProfile)
        {
            return new DirectoryPath(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile));
        }

        var result = GetXPlatFolderPath(platform, path);
        if (result != null)
        {
            return new DirectoryPath(result);
        }

        const string format = "The special path '{0}' is not supported.";
        throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, format, path));
    }

    private static string? GetXPlatFolderPath(IPlatform platform, KnownPath path)
    {
        if (platform.IsUnix())
        {
            return Native.Unix.GetFolder(path);
        }

        if (platform.Family == PlatformFamily.Windows)
        {
            return Native.Windows.GetFolder(path);
        }

        throw new PlatformNotSupportedException();
    }
}
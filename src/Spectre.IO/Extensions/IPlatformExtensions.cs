using Spectre.IO.Internal;

namespace Spectre.IO;

/// <summary>
/// Contains extension methods for <see cref="IPlatform"/>.
/// </summary>
[PublicAPI]
public static class IPlatformExtensions
{
    /// <summary>
    /// Determines whether the specified platform is a Unix platform.
    /// </summary>
    /// <param name="platform">The platform.</param>
    /// <returns><c>true</c> if the platform is a Unix platform; otherwise <c>false</c>.</returns>
    public static bool IsUnix(this IPlatform platform)
    {
        ArgumentNullException.ThrowIfNull(platform);

        return EnvironmentHelper.IsUnix(platform.Family);
    }
}
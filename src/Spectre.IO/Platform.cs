using Spectre.IO.Internal;

namespace Spectre.IO;

/// <summary>
/// Represents the platform that Cake is running on.
/// </summary>
[PublicAPI]
public sealed class Platform : IPlatform
{
    /// <inheritdoc/>
    public PlatformFamily Family { get; }

    /// <inheritdoc/>
    public PlatformArchitecture Architecture { get; }

    /// <inheritdoc/>
    [Obsolete("Use the Architecture property instead")]
    public bool Is64Bit => Architecture == PlatformArchitecture.X64;

    /// <summary>
    /// Gets the default <see cref="Platform"/> instance.
    /// </summary>
    public static Platform Shared { get; } = new Platform();

    /// <summary>
    /// Initializes a new instance of the <see cref="Platform"/> class.
    /// </summary>
    public Platform()
    {
        Family = EnvironmentHelper.GetPlatformFamily();
        Architecture = EnvironmentHelper.GetPlatformArchitecture();
    }

    /// <summary>
    /// Gets the platform family.
    /// </summary>
    /// <returns>The platform family.</returns>
    public static PlatformFamily GetPlatformFamily()
    {
        return EnvironmentHelper.GetPlatformFamily();
    }

    /// <summary>
    /// Gets the platform processor architecture.
    /// </summary>
    /// <returns>The platform processor architecture.</returns>
    public static PlatformArchitecture GetPlatformArchitecture()
    {
        return EnvironmentHelper.GetPlatformArchitecture();
    }

    /// <summary>
    /// Determines whether the current platform is a 64-bit operating system.
    /// </summary>
    /// <returns><c>true</c> if the current platform is a 64-bit operating system; otherwise <c>false</c>.</returns>
    [Obsolete("Use GetPlatformArchitecture instead")]
    public static bool Is64BitOperativeSystem()
    {
        return EnvironmentHelper.GetPlatformArchitecture() == PlatformArchitecture.X64;
    }

    /// <summary>
    /// Determines whether the current platform is a Unix platform.
    /// </summary>
    /// <returns><c>true</c> if the current platform is a Unix platform; otherwise <c>false</c>.</returns>
    [Obsolete("Use GetPlatformFamily instead")]
    public static bool IsUnix()
    {
        return IsUnix(EnvironmentHelper.GetPlatformFamily());
    }

    /// <summary>
    /// Determines whether the specified platform family is a Unix platform.
    /// </summary>
    /// <param name="family">The platform family.</param>
    /// <returns><c>true</c> if the platform is a Unix platform; otherwise <c>false</c>.</returns>
    [Obsolete("Use GetPlatformFamily instead")]
    public static bool IsUnix(PlatformFamily family)
    {
        return family == PlatformFamily.Linux
               || family == PlatformFamily.MacOs;
    }
}
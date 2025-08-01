namespace Spectre.IO;

/// <summary>
/// Represents the platform we're running on.
/// </summary>
[PublicAPI]
public interface IPlatform
{
    /// <summary>
    /// Gets the platform family.
    /// </summary>
    /// <value>The platform family.</value>
    PlatformFamily Family { get; }

    /// <summary>
    /// Gets the platform process architecture.
    /// </summary>
    /// <value>The platform process architecture.</value>
    PlatformArchitecture Architecture { get; }

    /// <summary>
    /// Gets a value indicating whether or not the current operative system is 64 bit.
    /// </summary>
    /// <value>
    ///   <c>true</c> if current operative system is 64 bit; otherwise, <c>false</c>.
    /// </value>
    [Obsolete("Use the Architecture property instead")]
    bool Is64Bit { get; }
}
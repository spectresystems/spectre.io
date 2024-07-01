namespace Spectre.IO;

/// <summary>
/// Represents a platform family.
/// </summary>
public enum PlatformFamily
{
    /// <summary>
    /// The platform family is unknown.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Represents the Windows platform family.
    /// </summary>
    Windows = 1,

    /// <summary>
    /// Represents the Linux platform family.
    /// </summary>
    Linux = 2,

    /// <summary>
    /// Represents the MacOs platform family.
    /// </summary>
    MacOs = 3,

    /// <summary>
    /// Represents the FreeBSD platform family.
    /// </summary>
    FreeBSD = 4,
}
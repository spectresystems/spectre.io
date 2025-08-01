namespace Spectre.IO.Testing;

/// <summary>
/// An implementation of a fake <see cref="IPlatform"/>.
/// </summary>
[PublicAPI]
public sealed class FakePlatform : IPlatform
{
    /// <inheritdoc/>
    public PlatformFamily Family { get; set; }

    /// <inheritdoc/>
    public PlatformArchitecture Architecture { get; set; }

    /// <inheritdoc/>
    public bool Is64Bit => Architecture == PlatformArchitecture.X64;

    /// <summary>
    /// Initializes a new instance of the <see cref="FakePlatform"/> class.
    /// </summary>
    /// <param name="family">The platform family.</param>
    /// <param name="architecture">The platform processor architecture.</param>
    public FakePlatform(PlatformFamily family, PlatformArchitecture architecture = PlatformArchitecture.X64)
    {
        Family = family;
        Architecture = architecture;
    }
}
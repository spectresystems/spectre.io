namespace Spectre.IO;

/// <summary>
/// Represents a processor architectures.
/// </summary>
[PublicAPI]
public enum PlatformArchitecture
{
    /// <summary>
    /// The processor architecture is unknown.
    /// </summary>
    Unknown,

    /// <summary>
    /// An Intel-based 32-bit processor architecture.
    /// </summary>
    X86 = 1,

    /// <summary>
    /// An Intel-based 64-bit processor architecture.
    /// </summary>
    X64 = 2,

    /// <summary>
    /// A 32-bit ARM processor architecture.
    /// </summary>
    Arm = 3,

    /// <summary>
    /// A 64-bit ARM processor architecture.
    /// </summary>
    Arm64 = 4,

    /// <summary>
    /// The WebAssembly platform.
    /// </summary>
    Wasm = 5,
}
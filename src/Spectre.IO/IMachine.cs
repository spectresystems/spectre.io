namespace Spectre.IO;

/// <summary>
/// Represents a "machine" which has a
/// filesystem, environment and a platform.
/// </summary>
[PublicAPI]
public interface IMachine
{
    /// <summary>
    /// Gets the machine's <see cref="IFileSystem"/>.
    /// </summary>
    IFileSystem FileSystem { get; }

    /// <summary>
    /// Gets the machine's <see cref="IEnvironment"/>.
    /// </summary>
    IEnvironment Environment { get; }

    /// <summary>
    /// Gets the machine's <see cref="IPlatform"/>.
    /// </summary>
    public IPlatform Platform => Environment.Platform;
}
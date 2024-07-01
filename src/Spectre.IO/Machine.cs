namespace Spectre.IO;

/// <summary>
/// An implementation of <see cref="IMachine"/> that
/// reflects the current machine.
/// </summary>
public sealed class Machine : IMachine
{
    /// <summary>
    /// Gets the default <see cref="FileSystem"/> instance.
    /// </summary>
    public static Machine Shared { get; } = new();

    /// <inheritdoc/>
    public IFileSystem FileSystem { get; }

    /// <inheritdoc/>
    public IEnvironment Environment { get; }

    /// <inheritdoc/>
    public IPlatform Platform { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Machine"/> class.
    /// </summary>
    /// <param name="fileSystem">The file system to use, or <c>null</c> to use the default one.</param>
    /// <param name="environment">The environment to use, or <c>null</c> to use the default one.</param>
    /// <param name="platform">The platform to use, or <c>null</c> to use the default one.</param>
    public Machine(
        IFileSystem? fileSystem = null,
        IEnvironment? environment = null,
        IPlatform? platform = null)
    {
        FileSystem = fileSystem ?? IO.FileSystem.Shared;
        Environment = environment ?? IO.Environment.Shared;
        Platform = platform ?? IO.Platform.Shared;
    }
}
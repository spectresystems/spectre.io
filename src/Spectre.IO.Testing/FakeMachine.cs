namespace Spectre.IO.Testing;

/// <summary>
/// Represents a fake machine.
/// </summary>
[PublicAPI]
public sealed class FakeMachine : IMachine
{
    /// <summary>
    /// Gets the fake file system.
    /// </summary>
    public FakeFileSystem FileSystem { get; }

    /// <summary>
    /// Gets the fake environment.
    /// </summary>
    public FakeEnvironment Environment { get; }

    /// <inheritdoc/>
    IFileSystem IMachine.FileSystem => FileSystem;

    /// <inheritdoc/>
    IEnvironment IMachine.Environment => Environment;

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeMachine"/> class.
    /// </summary>
    /// <param name="family">The platform family.</param>
    /// <param name="architecture">The platform processor architecture.</param>
    public FakeMachine(
        PlatformFamily family,
        PlatformArchitecture architecture = PlatformArchitecture.X64)
    {
        Environment = new FakeEnvironment(family, architecture);
        FileSystem = new FakeFileSystem(Environment);
    }

    /// <summary>
    /// Creates a Linux machine.
    /// </summary>
    /// <param name="architecture">The platform processor architecture.</param>
    /// <returns>A Linux environment.</returns>
    public static FakeMachine CreateLinuxMachine(PlatformArchitecture architecture = PlatformArchitecture.X64)
    {
        return new FakeMachine(PlatformFamily.Linux, architecture);
    }

    /// <summary>
    /// Creates a macOS environment.
    /// </summary>
    /// <param name="architecture">The platform processor architecture.</param>
    /// <returns>A macOS environment.</returns>
    public static FakeMachine CreateMacOsMachine(PlatformArchitecture architecture = PlatformArchitecture.X64)
    {
        return new FakeMachine(PlatformFamily.MacOs, architecture);
    }

    /// <summary>
    /// Creates a Windows environment.
    /// </summary>
    /// <param name="architecture">The platform processor architecture.</param>
    /// <returns>A Windows environment.</returns>
    public static FakeMachine CreateWindowsMachine(PlatformArchitecture architecture = PlatformArchitecture.X64)
    {
        return new FakeMachine(PlatformFamily.Windows, architecture);
    }

    /// <summary>
    /// Creates a FreeBSD environment.
    /// </summary>
    /// <param name="architecture">The platform processor architecture.</param>
    /// <returns>A Windows environment.</returns>
    public static FakeMachine CreateFreeBsdMachine(PlatformArchitecture architecture = PlatformArchitecture.X64)
    {
        return new FakeMachine(PlatformFamily.FreeBSD, architecture);
    }
}
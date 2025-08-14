namespace Spectre.IO.Testing;

/// <summary>
/// Represents a fake environment.
/// </summary>
[PublicAPI]
public sealed class FakeEnvironment : IEnvironment
{
    private readonly Dictionary<string, string?> _environmentVariables;

    /// <inheritdoc/>
    public DirectoryPath WorkingDirectory { get; private set; }

    /// <inheritdoc/>
    public DirectoryPath HomeDirectory { get; }

    /// <inheritdoc/>
    IPlatform IEnvironment.Platform => Platform;

    /// <summary>
    /// Gets the fake platform.
    /// </summary>
    /// <value>The fake platform.</value>
    public FakePlatform Platform { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeEnvironment"/> class.
    /// </summary>
    /// <param name="family">The platform family.</param>
    /// <param name="architecture">The platform processor architecture.</param>
    public FakeEnvironment(PlatformFamily family, PlatformArchitecture architecture = PlatformArchitecture.X64)
        : this(new FakePlatform(family, architecture))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeEnvironment"/> class.
    /// </summary>
    /// <param name="platform">The underlying platform for the environment.</param>
    public FakeEnvironment(FakePlatform platform)
    {
        _environmentVariables = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        Platform = platform ?? throw new ArgumentNullException(nameof(platform));

        switch (Platform.Family)
        {
            case PlatformFamily.Windows:
                WorkingDirectory = new DirectoryPath("C:/Working");
                HomeDirectory = new DirectoryPath("C:/Users/JohnDoe");
                break;
            case PlatformFamily.MacOs:
                WorkingDirectory = new DirectoryPath("/Working");
                HomeDirectory = new DirectoryPath("/Users/JohnDoe");
                break;
            case PlatformFamily.FreeBSD:
                WorkingDirectory = new DirectoryPath("/Working");
                HomeDirectory = new DirectoryPath("/usr/home/JohnDoe");
                break;
            case PlatformFamily.Linux:
                WorkingDirectory = new DirectoryPath("/Working");
                HomeDirectory = new DirectoryPath("/home/JohnDoe");
                break;
            default:
                throw new ArgumentException("Unknown platform family", nameof(platform));
        }
    }

    /// <summary>
    /// Creates a Linux environment.
    /// </summary>
    /// <param name="architecture">The platform processor architecture.</param>
    /// <returns>A Linux environment.</returns>
    public static FakeEnvironment CreateLinuxEnvironment(PlatformArchitecture architecture = PlatformArchitecture.X64)
    {
        return new FakeEnvironment(PlatformFamily.Linux, architecture);
    }

    /// <summary>
    /// Creates a macOS environment.
    /// </summary>
    /// <param name="architecture">The platform processor architecture.</param>
    /// <returns>A macOS environment.</returns>
    public static FakeEnvironment CreateMacOSEnvironment(PlatformArchitecture architecture = PlatformArchitecture.X64)
    {
        return new FakeEnvironment(PlatformFamily.MacOs, architecture);
    }

    /// <summary>
    /// Creates a Windows environment.
    /// </summary>
    /// <param name="architecture">The platform processor architecture.</param>
    /// <returns>A Windows environment.</returns>
    public static FakeEnvironment CreateWindowsEnvironment(PlatformArchitecture architecture = PlatformArchitecture.X64)
    {
        return new FakeEnvironment(PlatformFamily.Windows, architecture);
    }

    /// <summary>
    /// Creates a FreeBSD environment.
    /// </summary>
    /// <param name="architecture">The platform processor architecture.</param>
    /// <returns>A Windows environment.</returns>
    public static FakeEnvironment CreateFreeBSDEnvironment(PlatformArchitecture architecture = PlatformArchitecture.X64)
    {
        return new FakeEnvironment(PlatformFamily.FreeBSD, architecture);
    }

    /// <inheritdoc/>
    public string? GetEnvironmentVariable(string variable)
    {
        return _environmentVariables.GetValueOrDefault(variable);
    }

    /// <inheritdoc/>
    public IDictionary<string, string?> GetEnvironmentVariables()
    {
        return new Dictionary<string, string?>(_environmentVariables, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Creates a Unix environment.
    /// </summary>
    /// <param name="architecture">The platform processor architecture.</param>
    /// <returns>A Unix environment.</returns>
    [Obsolete("Use CreateLinuxEnvironment instead")]
    public static FakeEnvironment CreateUnixEnvironment(PlatformArchitecture architecture = PlatformArchitecture.X64)
    {
        return new FakeEnvironment(PlatformFamily.Linux, architecture);
    }

    /// <summary>
    /// Changess the operative system bitness.
    /// </summary>
    /// <param name="is64Bit">if set to <c>true</c>, this is a 64-bit operative system.</param>
    [Obsolete("Use ChangePlatformArchitectureInstead")]
    public void ChangeOperativeSystemBitness(bool is64Bit)
    {
        Platform.Architecture = is64Bit
            ? PlatformArchitecture.X64
            : PlatformArchitecture.X86;
    }

    /// <summary>
    /// Changes the operating system platform family.
    /// </summary>
    /// <param name="family">The platform family.</param>
    [Obsolete("Use ChangePlatformFamilyInstead")]
    public void ChangeOperatingSystemFamily(PlatformFamily family)
    {
        Platform.Family = family;
    }

    /// <summary>
    /// Changes the platform family.
    /// </summary>
    /// <param name="family">The platform family.</param>
    [Obsolete("Use ChangePlatformFamilyInstead")]
    public void ChangePlatformSystemFamily(PlatformFamily family)
    {
        Platform.Family = family;
    }

    /// <summary>
    /// Changes the platform processor architecture.
    /// </summary>
    /// <param name="architecture">The platform processor architecture.</param>
    public void ChangePlatformArchitecture(PlatformArchitecture architecture)
    {
        Platform.Architecture = architecture;
    }

    /// <summary>
    /// Sets an environment variable.
    /// </summary>
    /// <param name="variable">The variable.</param>
    /// <param name="value">The value.</param>
    public void SetEnvironmentVariable(string variable, string value)
    {
        _environmentVariables[variable] = value;
    }

    /// <inheritdoc/>
    public void SetWorkingDirectory(DirectoryPath path)
    {
        ArgumentNullException.ThrowIfNull(path);

        WorkingDirectory = path;
    }

    /// <inheritdoc/>
    public DirectoryPath GetTempDirectory()
    {
        return Platform.Family switch
        {
            PlatformFamily.MacOs => "/var/folders/tmp",
            PlatformFamily.Windows => HomeDirectory.Combine("AppData/Local/Temp"),
            _ => "/tmp",
        };
    }
}
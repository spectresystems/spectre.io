namespace Spectre.IO.Testing;

/// <summary>
/// Represents a fake environment.
/// </summary>
[PublicAPI]
public sealed class FakeEnvironment : IEnvironment
{
    private readonly Dictionary<string, string?> _environmentVariables;
    private readonly Dictionary<KnownPath, DirectoryPath> _knownPaths;

    private static readonly Dictionary<PlatformFamily, Dictionary<KnownPath, DirectoryPath>> _defaultKnownPaths = new()
    {
        [PlatformFamily.Unknown] = new(),
        [PlatformFamily.Windows] = new()
        {
            [KnownPath.ApplicationData] = new("C:/Users/JohnDoe/AppData/Roaming"),
            [KnownPath.CommonApplicationData] = new("C:/ProgramData"),
            [KnownPath.LocalApplicationData] = new("C:/Users/JohnDoe/AppData/Local"),
            [KnownPath.ProgramFiles] = new("C:/Program Files"),
            [KnownPath.ProgramFilesX86] = new("C:/Program Files (x86)"),
            [KnownPath.Windows] = new("C:/Windows"),
            [KnownPath.LocalTemp] = new("C:/Users/JohnDoe/AppData/Local/Temp"),
            [KnownPath.UserProfile] = new("C:/Users/JohnDoe"),
        },
        [PlatformFamily.Linux] = new()
        {
            [KnownPath.ApplicationData] = new("/home/JohnDoe/.config"),
            [KnownPath.CommonApplicationData] = new("/var/lib"),
            [KnownPath.LocalApplicationData] = new("/home/JohnDoe/.local/share"),
            [KnownPath.LocalTemp] = new("/tmp"),
            [KnownPath.UserProfile] = new("/home/JohnDoe"),
        },
        [PlatformFamily.MacOs] = new()
        {
            [KnownPath.ApplicationData] = new("/Users/JohnDoe/Library/Application Support"),
            [KnownPath.CommonApplicationData] = new("/Library/Application Support"),
            [KnownPath.LocalApplicationData] = new("/Users/JohnDoe/Library/Application Support"),
            [KnownPath.LocalTemp] = new("/tmp"),
            [KnownPath.UserProfile] = new("/Users/JohnDoe"),
        },
        [PlatformFamily.FreeBsd] = new()
        {
            [KnownPath.ApplicationData] = new("/home/JohnDoe/.config"),
            [KnownPath.CommonApplicationData] = new("/var/db"),
            [KnownPath.LocalApplicationData] = new("/home/JohnDoe/.local/share"),
            [KnownPath.LocalTemp] = new("/tmp"),
            [KnownPath.UserProfile] = new("/home/JohnDoe"),
        },
    };

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
        _knownPaths = new Dictionary<KnownPath, DirectoryPath>();

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
            case PlatformFamily.FreeBsd:
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

        // Create all known paths
        foreach (var path in _defaultKnownPaths[platform.Family])
        {
            _knownPaths[path.Key] = path.Value;
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
    public static FakeEnvironment CreateMacOsEnvironment(PlatformArchitecture architecture = PlatformArchitecture.X64)
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
    public static FakeEnvironment CreateFreeBsdEnvironment(PlatformArchitecture architecture = PlatformArchitecture.X64)
    {
        return new FakeEnvironment(PlatformFamily.FreeBsd, architecture);
    }

    /// <inheritdoc/>
    public string? GetEnvironmentVariable(string variable)
    {
        return _environmentVariables.GetValueOrDefault(variable);
    }

    /// <inheritdoc/>
    public DirectoryPath GetKnownPath(KnownPath path)
    {
        return _knownPaths[path];
    }

    /// <summary>
    /// Gets all known paths.
    /// </summary>
    /// <returns>All known paths.</returns>
    public IEnumerable<DirectoryPath> GetKnownPaths()
    {
        return _knownPaths.Values;
    }

    /// <summary>
    /// Sets a known path.
    /// </summary>
    /// <param name="kind">The known path kind.</param>
    /// <param name="path">The path.</param>
    public void SetKnownPath(KnownPath kind, DirectoryPath path)
    {
        _knownPaths[kind] = path;
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
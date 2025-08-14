using SystemDirectory = System.IO.Directory;
using SystemEnv = System.Environment;
using SystemFolder = System.Environment.SpecialFolder;

namespace Spectre.IO;

/// <summary>
/// Represents the environment.
/// </summary>
[PublicAPI]
public sealed class Environment : IEnvironment
{
    /// <summary>
    /// Gets the default <see cref="Environment"/> instance.
    /// </summary>
    public static Environment Shared { get; } = new Environment();

    /// <inheritdoc/>
    public DirectoryPath WorkingDirectory
    {
        get { return new DirectoryPath(SystemDirectory.GetCurrentDirectory()); }
    }

    /// <inheritdoc/>
    public DirectoryPath HomeDirectory
    {
        get { return new DirectoryPath(SystemEnv.GetFolderPath(SystemFolder.UserProfile)); }
    }

    /// <inheritdoc/>
    public IPlatform Platform { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Environment"/> class.
    /// </summary>
    public Environment()
        : this(Spectre.IO.Platform.Shared)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Environment" /> class.
    /// </summary>
    /// <param name="platform">The platform.</param>
    public Environment(IPlatform platform)
    {
        Platform = platform;

        SetWorkingDirectory(new DirectoryPath(SystemDirectory.GetCurrentDirectory()));
    }

    /// <inheritdoc/>
    public string? GetEnvironmentVariable(string variable)
    {
        return SystemEnv.GetEnvironmentVariable(variable);
    }

    /// <inheritdoc/>
    public IDictionary<string, string?> GetEnvironmentVariables()
    {
        return SystemEnv.GetEnvironmentVariables()
            .Cast<System.Collections.DictionaryEntry>()
            .Aggregate(
                new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase),
                (dictionary, entry) =>
                {
                    var key = (string)entry.Key;
                    if (!dictionary.TryGetValue(key, out _))
                    {
                        dictionary.Add(key, entry.Value as string);
                    }

                    return dictionary;
                },
                dictionary => dictionary);
    }

    /// <inheritdoc/>
    public void SetWorkingDirectory(DirectoryPath path)
    {
        ArgumentNullException.ThrowIfNull(path);

        if (path.IsRelative)
        {
            throw new IOException("Working directory can not be set to a relative path.");
        }

        SystemDirectory.SetCurrentDirectory(path.FullPath);
    }

    /// <inheritdoc/>
    public DirectoryPath GetTempDirectory()
    {
        var path = System.IO.Path.GetTempPath();
        return new DirectoryPath(path);
    }
}
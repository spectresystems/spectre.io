namespace Spectre.IO;

/// <summary>
/// Represents the environment.
/// </summary>
[PublicAPI]
public interface IEnvironment
{
    /// <summary>
    /// Gets the working directory.
    /// </summary>
    DirectoryPath WorkingDirectory { get; }

    /// <summary>
    /// Gets the users home directory.
    /// </summary>
    DirectoryPath HomeDirectory { get; }

    /// <summary>
    /// Gets the platform we're running on.
    /// </summary>
    IPlatform Platform { get; }

    /// <summary>
    /// Gets an environment variable.
    /// </summary>
    /// <param name="variable">The variable.</param>
    /// <returns>The value of the environment variable.</returns>
    string? GetEnvironmentVariable(string variable);

    /// <summary>
    /// Gets all environment variables.
    /// </summary>
    /// <returns>The environment variables as IDictionary&lt;string, string&gt;. </returns>
    IDictionary<string, string?> GetEnvironmentVariables();

    /// <summary>
    /// Sets the working directory.
    /// </summary>
    /// <param name="path">The path to use as the working directory.</param>
    void SetWorkingDirectory(DirectoryPath path);

    /// <summary>
    /// Returns the path of the current user's temporary folder.
    /// </summary>
    /// <returns>The path to the temporary folder.</returns>
    DirectoryPath GetTempDirectory();
}
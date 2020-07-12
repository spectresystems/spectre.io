using System.Collections.Generic;

namespace Spectre.IO
{
    /// <summary>
    /// Represents the environment.
    /// </summary>
    public interface IEnvironment
    {
        /// <summary>
        /// Gets the working directory.
        /// </summary>
        /// <value>The working directory.</value>
        DirectoryPath WorkingDirectory { get; }

        /// <summary>
        /// Gets the platform we're running on.
        /// </summary>
        /// <value>The platform we're running on.</value>
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
    }
}
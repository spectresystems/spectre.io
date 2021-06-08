using System;

namespace Spectre.IO
{
    /// <summary>
    /// Contains extensions for <see cref="FilePath"/>.
    /// </summary>
    public static class FilePathExtensions
    {
        /// <summary>
        /// Expands all environment variables in the provided <see cref="FilePath"/>.
        /// </summary>
        /// <example>
        /// <code>
        /// var path = new FilePath("%APPDATA%/foo.bar");
        /// var expanded = path.ExpandEnvironmentVariables(environment);
        /// </code>
        /// </example>
        /// <param name="path">The file path to expand.</param>
        /// <param name="environment">The environment.</param>
        /// <returns>A new <see cref="FilePath"/> with each environment variable replaced by its value.</returns>
        public static FilePath ExpandEnvironmentVariables(this FilePath path, IEnvironment environment)
        {
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            var result = environment.ExpandEnvironmentVariables(path.FullPath);
            return new FilePath(result);
        }
    }
}

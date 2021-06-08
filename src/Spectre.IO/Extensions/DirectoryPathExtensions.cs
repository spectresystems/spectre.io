using System;

namespace Spectre.IO
{
    /// <summary>
    /// Contains extensions for <see cref="DirectoryPath"/>.
    /// </summary>
    public static class DirectoryPathExtensions
    {
        /// <summary>
        /// Expands all environment variables in the provided <see cref="DirectoryPath"/>.
        /// </summary>
        /// <example>
        /// <code>
        /// var path = new DirectoryPath("%APPDATA%");
        /// var expanded = path.ExpandEnvironmentVariables(environment);
        /// </code>
        /// </example>
        /// <param name="path">The directory to expand.</param>
        /// <param name="environment">The environment.</param>
        /// <returns>A new <see cref="DirectoryPath"/> with each environment variable replaced by its value.</returns>
        public static DirectoryPath ExpandEnvironmentVariables(this DirectoryPath path, IEnvironment environment)
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
            return new DirectoryPath(result);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Spectre.IO
{
    /// <summary>
    /// Represents the environment.
    /// </summary>
    public sealed class Environment : IEnvironment
    {
        /// <inheritdoc/>
        public DirectoryPath WorkingDirectory
        {
            get { return new DirectoryPath(System.IO.Directory.GetCurrentDirectory()); }
        }

        /// <inheritdoc/>
        public IPlatform Platform { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Environment"/> class.
        /// </summary>
        public Environment()
            : this(new Platform())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Environment" /> class.
        /// </summary>
        /// <param name="platform">The platform.</param>
        public Environment(IPlatform platform)
        {
            Platform = platform;

            SetWorkingDirectory(new DirectoryPath(System.IO.Directory.GetCurrentDirectory()));
        }

        /// <inheritdoc/>
        public string? GetEnvironmentVariable(string variable)
        {
            return System.Environment.GetEnvironmentVariable(variable);
        }

        /// <inheritdoc/>
        public IDictionary<string, string?> GetEnvironmentVariables()
        {
            return System.Environment.GetEnvironmentVariables()
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
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (path.IsRelative)
            {
                throw new IOException("Working directory can not be set to a relative path.");
            }

            System.IO.Directory.SetCurrentDirectory(path.FullPath);
        }
    }
}
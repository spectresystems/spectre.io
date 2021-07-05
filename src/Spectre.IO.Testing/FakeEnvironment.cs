using System;
using System.Collections.Generic;

namespace Spectre.IO.Testing
{
    /// <summary>
    /// Represents a fake environment.
    /// </summary>
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
        /// <param name="is64Bit">if set to <c>true</c>, the platform is 64 bit.</param>
        public FakeEnvironment(PlatformFamily family, bool is64Bit = true)
        {
            _environmentVariables = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

            if (family == PlatformFamily.Windows)
            {
                WorkingDirectory = new DirectoryPath("C:/Working");
                HomeDirectory = new DirectoryPath("C:/Users/Patrik");
            }
            else
            {
                WorkingDirectory = new DirectoryPath("/Working");
                HomeDirectory = new DirectoryPath("/home/Patrik");
            }

            Platform = new FakePlatform(family, is64Bit);
        }

        /// <summary>
        /// Creates a Unix environment.
        /// </summary>
        /// <param name="is64Bit">if set to <c>true</c> the platform is 64 bit.</param>
        /// <returns>A Unix environment.</returns>
        public static FakeEnvironment CreateUnixEnvironment(bool is64Bit = true)
        {
            return new FakeEnvironment(PlatformFamily.Linux, is64Bit);
        }

        /// <summary>
        /// Creates a Windows environment.
        /// </summary>
        /// <param name="is64Bit">if set to <c>true</c> the platform is 64 bit.</param>
        /// <returns>A Windows environment.</returns>
        public static FakeEnvironment CreateWindowsEnvironment(bool is64Bit = true)
        {
            return new FakeEnvironment(PlatformFamily.Windows, is64Bit);
        }

        /// <inheritdoc/>
        public string? GetEnvironmentVariable(string variable)
        {
            if (_environmentVariables.ContainsKey(variable))
            {
                return _environmentVariables[variable];
            }

            return null;
        }

        /// <inheritdoc/>
        public IDictionary<string, string?> GetEnvironmentVariables()
        {
            return new Dictionary<string, string?>(_environmentVariables, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Changes the operative system bitness.
        /// </summary>
        /// <param name="is64Bit">if set to <c>true</c>, this is a 64-bit operative system.</param>
        public void ChangeOperativeSystemBitness(bool is64Bit)
        {
            Platform.Is64Bit = is64Bit;
        }

        /// <summary>
        /// Change the operating system platform family.
        /// </summary>
        /// <param name="family">The platform family.</param>
        public void ChangeOperatingSystemFamily(PlatformFamily family)
        {
            Platform.Family = family;
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
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            WorkingDirectory = path;
        }
    }
}
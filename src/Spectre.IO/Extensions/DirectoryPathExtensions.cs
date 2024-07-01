using System;
using System.Collections.Generic;

namespace Spectre.IO;

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

    /// <summary>
    /// Converts an <see cref="IEnumerable{DirectoryPath}"/> to a <see cref="DirectoryPathCollection"/>.
    /// </summary>
    /// <param name="source">The paths to add to the collection.</param>
    /// <param name="comparer">The comparer to use. If <c>null</c>, the default one is used.</param>
    /// <returns>A new <see cref="DirectoryPathCollection"/>.</returns>
    public static DirectoryPathCollection ToPathCollection(this IEnumerable<DirectoryPath> source, IPathComparer? comparer = null)
    {
        return new DirectoryPathCollection(source, comparer);
    }
}
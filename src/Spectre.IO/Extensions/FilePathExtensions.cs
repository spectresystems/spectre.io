namespace Spectre.IO;

/// <summary>
/// Contains extensions for <see cref="FilePath"/>.
/// </summary>
[PublicAPI]
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
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(environment);

        var result = environment.ExpandEnvironmentVariables(path.FullPath);
        return new FilePath(result);
    }

    /// <summary>
    /// Converts an <see cref="IEnumerable{FilePath}"/> to a <see cref="FilePathCollection"/>.
    /// </summary>
    /// <param name="source">The paths to add to the collection.</param>
    /// <param name="comparer">The comparer to use. If <c>null</c>, the default one is used.</param>
    /// <returns>A new <see cref="FilePathCollection"/>.</returns>
    public static FilePathCollection ToPathCollection(this IEnumerable<FilePath> source, IPathComparer? comparer = null)
    {
        return new FilePathCollection(source, comparer);
    }
}
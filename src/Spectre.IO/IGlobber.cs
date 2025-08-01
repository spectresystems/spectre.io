namespace Spectre.IO;

/// <summary>
/// Represents a file system globber.
/// </summary>
[PublicAPI]
public interface IGlobber
{
    /// <summary>
    /// Returns <see cref="Path" /> instances matching the specified pattern.
    /// </summary>
    /// <param name="pattern">The pattern to match.</param>
    /// <param name="settings">The globber settings.</param>
    /// <returns>
    ///   <see cref="Path" /> instances matching the specified pattern.
    /// </returns>
    IEnumerable<Path> Match(string pattern, GlobberSettings settings);
}
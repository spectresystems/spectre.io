using System;
using System.Collections.Generic;
using System.Linq;

namespace Spectre.IO;

/// <summary>
/// Contains extensions for <see cref="IGlobber"/>.
/// </summary>
public static class IGlobberExtensions
{
    /// <summary>
    /// Gets all files matching the specified pattern.
    /// </summary>
    /// <param name="globber">The globber.</param>
    /// <param name="pattern">The pattern.</param>
    /// <returns>The files matching the specified pattern.</returns>
    public static IEnumerable<FilePath> GetFiles(this IGlobber globber, string pattern)
    {
        if (globber == null)
        {
            throw new ArgumentNullException(nameof(globber));
        }

        return globber.Match(pattern).OfType<FilePath>();
    }

    /// <summary>
    /// Gets all directories matching the specified pattern.
    /// </summary>
    /// <param name="globber">The globber.</param>
    /// <param name="pattern">The pattern.</param>
    /// <returns>The directories matching the specified pattern.</returns>
    public static IEnumerable<DirectoryPath> GetDirectories(this IGlobber globber, string pattern)
    {
        if (globber == null)
        {
            throw new ArgumentNullException(nameof(globber));
        }

        return globber.Match(pattern).OfType<DirectoryPath>();
    }

    /// <summary>
    /// Returns <see cref="Path" /> instances matching the specified pattern.
    /// </summary>
    /// <param name="globber">The globber.</param>
    /// <param name="pattern">The pattern to match.</param>
    /// <returns>
    ///   <see cref="Path" /> instances matching the specified pattern.
    /// </returns>
    public static IEnumerable<Path> Match(this IGlobber globber, string pattern)
    {
        if (globber == null)
        {
            throw new ArgumentNullException(nameof(globber));
        }

        return globber.Match(pattern, new GlobberSettings());
    }
}
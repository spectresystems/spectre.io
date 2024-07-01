using System.Collections.Generic;

namespace Spectre.IO;

/// <summary>
/// Contains extensions for <see cref="Path"/>.
/// </summary>
public static class PathExtensions
{
    /// <summary>
    /// Converts an <see cref="IEnumerable{Path}"/> to a <see cref="PathCollection"/>.
    /// </summary>
    /// <param name="source">The paths to add to the collection.</param>
    /// <param name="comparer">The comparer to use. If <c>null</c>, the default one is used.</param>
    /// <returns>A new <see cref="PathCollection"/>.</returns>
    public static PathCollection ToPathCollection(this IEnumerable<Path> source, IPathComparer? comparer = null)
    {
        return new PathCollection(source, comparer);
    }
}
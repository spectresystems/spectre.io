using System.Collections.Generic;

namespace Spectre.IO;

/// <summary>
/// Compares <see cref="Path"/> instances.
/// </summary>
public interface IPathComparer : IEqualityComparer<Path?>, IComparer<Path?>
{
    /// <summary>
    /// Gets a value indicating whether comparison is case sensitive.
    /// </summary>
    /// <value>
    /// <c>true</c> if comparison is case sensitive; otherwise, <c>false</c>.
    /// </value>
    bool IsCaseSensitive { get; }
}
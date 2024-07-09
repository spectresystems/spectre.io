using System;
using System.Linq;
using Spectre.IO.Internal;

namespace Spectre.IO;

/// <summary>
/// Compares <see cref="Path"/> instances.
/// </summary>
public sealed class PathComparer : IPathComparer
{
    /// <summary>
    /// Gets the default path comparer.
    /// </summary>
    public static PathComparer Default { get; }
        = new PathComparer(EnvironmentHelper.IsUnix());

    /// <summary>
    /// Gets a value indicating whether comparison is case sensitive.
    /// </summary>
    /// <value>
    /// <c>true</c> if comparison is case sensitive; otherwise, <c>false</c>.
    /// </value>
    public bool IsCaseSensitive { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PathComparer"/> class.
    /// </summary>
    /// <param name="caseSensitive">if set to <c>true</c>, comparison is case sensitive.</param>
    public PathComparer(bool caseSensitive)
    {
        IsCaseSensitive = caseSensitive;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PathComparer"/> class.
    /// </summary>
    /// <param name="environment">The environment.</param>
    public PathComparer(IEnvironment environment)
    {
        if (environment == null)
        {
            throw new ArgumentNullException(nameof(environment));
        }

        IsCaseSensitive = environment.Platform.IsUnix();
    }

    /// <inheritdoc/>
    public int Compare(Path? x, Path? y)
    {
        if (x == null && y == null)
        {
            return 0;
        }

        // This might look strange,
        // but for some reason, nullable reference tracking
        // does not work correctly otherwise.
        if (x == null || y == null)
        {
            if (x != null && y == null)
            {
                return -1;
            }

            return 1;
        }

        if (x.GetType() != y.GetType())
        {
            return -1;
        }

        if (x.Segments.Count != y.Segments.Count)
        {
            return x.Segments.Count
                .CompareTo(y.Segments.Count);
        }

        var comparer = IsCaseSensitive
            ? StringComparer.Ordinal
            : StringComparer.OrdinalIgnoreCase;

        foreach (var (segmentX, segmentY) in x.Segments.Zip(y.Segments))
        {
            var sort = comparer.Compare(segmentX, segmentY);
            if (sort != 0)
            {
                return sort;
            }
        }

        return 0;
    }

    /// <inheritdoc/>
    public bool Equals(Path? x, Path? y)
    {
        if (x == null && y == null)
        {
            return true;
        }

        if (x == null || y == null)
        {
            return false;
        }

        if (x.GetType() != y.GetType())
        {
            return false;
        }

        if (x.Segments.Count != y.Segments.Count)
        {
            return false;
        }

        var comparer = IsCaseSensitive
            ? StringComparer.Ordinal
            : StringComparer.OrdinalIgnoreCase;

        foreach (var (segmentX, segmentY) in x.Segments.Zip(y.Segments))
        {
            if (!comparer.Equals(segmentX, segmentY))
            {
                return false;
            }
        }

        return true;
    }

    /// <inheritdoc/>
    public int GetHashCode(Path? obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        if (IsCaseSensitive)
        {
            return obj.FullPath.GetHashCode();
        }

        return obj.FullPath.ToUpperInvariant().GetHashCode();
    }
}
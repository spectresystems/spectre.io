using System;
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
    public static PathComparer Default { get; } = new PathComparer(EnvironmentHelper.IsUnix());

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
    /// <param name="isCaseSensitive">if set to <c>true</c>, comparison is case sensitive.</param>
    public PathComparer(bool isCaseSensitive)
    {
        IsCaseSensitive = isCaseSensitive;
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

        if (x != null && y == null)
        {
            return -1;
        }

        if (x == null && y != null)
        {
            return 1;
        }

        if (IsCaseSensitive)
        {
            return StringComparer.Ordinal.Compare(
                x!.FullPath,
                y!.FullPath);
        }

        return StringComparer.OrdinalIgnoreCase.Compare(
            x!.FullPath,
            y!.FullPath);
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

        if (IsCaseSensitive)
        {
            return x.FullPath.Equals(y.FullPath, StringComparison.Ordinal);
        }

        return x.FullPath.Equals(y.FullPath, StringComparison.OrdinalIgnoreCase);
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
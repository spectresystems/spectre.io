using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Spectre.IO;

/// <summary>
/// A collection of <see cref="Path"/>.
/// </summary>
[PublicAPI]
public sealed class PathCollection : IEnumerable<Path>
{
    private readonly HashSet<Path> _paths;
    private readonly IPathComparer _comparer;

    /// <summary>
    /// Gets the number of paths in the collection.
    /// </summary>
    /// <value>The number of paths in the collection.</value>
    public int Count => _paths.Count;

    /// <summary>
    /// Initializes a new instance of the <see cref="PathCollection"/> class.
    /// </summary>
    public PathCollection()
        : this([], null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PathCollection"/> class.
    /// </summary>
    /// <param name="comparer">The comparer.</param>
    public PathCollection(IPathComparer comparer)
        : this([], comparer)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PathCollection"/> class.
    /// </summary>
    /// <param name="paths">The paths.</param>
    public PathCollection(IEnumerable<Path> paths)
        : this(paths, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PathCollection"/> class.
    /// </summary>
    /// <param name="paths">The paths.</param>
    /// <param name="comparer">The comparer.</param>
    /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is <c>null</c>.</exception>
    public PathCollection(IEnumerable<Path> paths, IPathComparer? comparer = null)
    {
        ArgumentNullException.ThrowIfNull(paths);

        _comparer = comparer ?? PathComparer.Default;
        _paths = new HashSet<Path>(paths, comparer);
    }

    /// <summary>
    /// Adds the specified path to the collection.
    /// </summary>
    /// <param name="path">The path to add.</param>
    /// <returns>
    ///   <c>true</c> if the path was added; <c>false</c> if the path was already present.
    /// </returns>
    public bool Add(Path path)
    {
        return _paths.Add(path);
    }

    /// <summary>
    /// Adds the specified paths to the collection.
    /// </summary>
    /// <param name="paths">The paths to add.</param>
    public void Add(IEnumerable<Path> paths)
    {
        ArgumentNullException.ThrowIfNull(paths);

        foreach (var path in paths)
        {
            _paths.Add(path);
        }
    }

    /// <summary>
    /// Removes the specified path from the collection.
    /// </summary>
    /// <param name="path">The path to remove.</param>
    /// <returns>
    ///   <c>true</c> if the path was removed; <c>false</c> if the path was not found in the collection.
    /// </returns>
    public bool Remove(Path path)
    {
        return _paths.Remove(path);
    }

    /// <summary>
    /// Removes the specified paths from the collection.
    /// </summary>
    /// <param name="paths">The paths to remove.</param>
    public void Remove(IEnumerable<Path> paths)
    {
        ArgumentNullException.ThrowIfNull(paths);

        foreach (var path in paths)
        {
            _paths.Remove(path);
        }
    }

    /// <summary>Adds a path to the collection.</summary>
    /// <param name="collection">The collection.</param>
    /// <param name="path">The path to add.</param>
    /// <returns>A new <see cref="PathCollection"/> that contains the provided path as
    /// well as the paths in the original collection.</returns>
    public static PathCollection operator +(PathCollection collection, Path path)
    {
        ArgumentNullException.ThrowIfNull(collection);

        return new PathCollection(collection, collection._comparer) { path };
    }

    /// <summary>Adds multiple paths to the collection.</summary>
    /// <param name="collection">The collection.</param>
    /// <param name="paths">The paths to add.</param>
    /// <returns>A new <see cref="PathCollection"/> with the content of both collections.</returns>
    public static PathCollection operator +(PathCollection collection, IEnumerable<Path> paths)
    {
        ArgumentNullException.ThrowIfNull(collection);

        return new PathCollection(collection, collection._comparer) { paths };
    }

    /// <summary>
    /// Removes a path from the collection.
    /// </summary>
    /// <param name="collection">The collection.</param>
    /// <param name="path">The path to remove.</param>
    /// <returns>A new <see cref="PathCollection"/> that do not contain the provided path.</returns>
    [SuppressMessage("Usage", "CA2225:Operator overloads have named alternates")]
    public static PathCollection operator -(PathCollection collection, Path path)
    {
        ArgumentNullException.ThrowIfNull(collection);

        var result = new PathCollection(collection, collection._comparer);
        result.Remove(path);
        return result;
    }

    /// <summary>
    /// Removes multiple paths from the collection.
    /// </summary>
    /// <param name="collection">The collection.</param>
    /// <param name="paths">The paths to remove.</param>
    /// <returns>A new <see cref="PathCollection"/> that do not contain the provided paths.</returns>
    [SuppressMessage("Usage", "CA2225:Operator overloads have named alternates")]
    public static PathCollection operator -(PathCollection collection, IEnumerable<Path> paths)
    {
        ArgumentNullException.ThrowIfNull(collection);

        var result = new PathCollection(collection, collection._comparer);
        result.Remove(paths);
        return result;
    }

    /// <inheritdoc/>
    public IEnumerator<Path> GetEnumerator()
    {
        return _paths.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
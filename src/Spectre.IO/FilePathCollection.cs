using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Spectre.IO
{
    /// <summary>
    /// A collection of <see cref="FilePath"/>.
    /// </summary>
    public sealed class FilePathCollection : IEnumerable<FilePath>
    {
        private readonly HashSet<FilePath> _paths;

        /// <summary>
        /// Gets the number of files in the collection.
        /// </summary>
        /// <value>The number of files in the collection.</value>
        public int Count => _paths.Count;

        internal PathComparer Comparer { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilePathCollection"/> class.
        /// </summary>
        public FilePathCollection()
            : this(PathComparer.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilePathCollection"/> class.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        public FilePathCollection(PathComparer comparer)
            : this(Enumerable.Empty<FilePath>(), comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilePathCollection"/> class.
        /// </summary>
        /// <param name="paths">The paths.</param>
        public FilePathCollection(IEnumerable<FilePath> paths)
            : this(paths, PathComparer.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilePathCollection"/> class.
        /// </summary>
        /// <param name="paths">The paths.</param>
        /// <param name="comparer">The comparer.</param>
        /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is <c>null</c>.</exception>
        public FilePathCollection(IEnumerable<FilePath> paths, PathComparer comparer)
        {
            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            _paths = new HashSet<FilePath>(paths, comparer);
        }

        /// <summary>
        /// Adds the specified path to the collection.
        /// </summary>
        /// <param name="path">The path to add.</param>
        /// <returns>
        ///   <c>true</c> if the path was added; <c>false</c> if the path was already present.
        /// </returns>
        public bool Add(FilePath path)
        {
            return _paths.Add(path);
        }

        /// <summary>
        /// Adds the specified paths to the collection.
        /// </summary>
        /// <param name="paths">The paths to add.</param>
        public void Add(IEnumerable<FilePath> paths)
        {
            if (paths == null)
            {
                throw new ArgumentNullException(nameof(paths));
            }

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
        public bool Remove(FilePath path)
        {
            return _paths.Remove(path);
        }

        /// <summary>
        /// Removes the specified paths from the collection.
        /// </summary>
        /// <param name="paths">The paths to remove.</param>
        public void Remove(IEnumerable<FilePath> paths)
        {
            if (paths == null)
            {
                throw new ArgumentNullException(nameof(paths));
            }

            foreach (var path in paths)
            {
                _paths.Remove(path);
            }
        }

        /// <summary>Adds a path to the collection.</summary>
        /// <param name="collection">The collection.</param>
        /// <param name="path">The path to add.</param>
        /// <returns>A new <see cref="FilePathCollection"/> that contains the provided path as
        /// well as the paths in the original collection.</returns>
        public static FilePathCollection operator +(FilePathCollection collection, FilePath path)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            return new FilePathCollection(collection, collection.Comparer) { path };
        }

        /// <summary>Adds multiple paths to the collection.</summary>
        /// <param name="collection">The collection.</param>
        /// <param name="paths">The paths to add.</param>
        /// <returns>A new <see cref="FilePathCollection"/> with the content of both collections.</returns>
        public static FilePathCollection operator +(FilePathCollection collection, IEnumerable<FilePath> paths)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            return new FilePathCollection(collection, collection.Comparer) { paths };
        }

        /// <summary>
        /// Removes a path from the collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="path">The path to remove.</param>
        /// <returns>A new <see cref="FilePathCollection"/> that do not contain the provided path.</returns>
        [SuppressMessage("Usage", "CA2225:Operator overloads have named alternates")]
        public static FilePathCollection operator -(FilePathCollection collection, FilePath path)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            var result = new FilePathCollection(collection, collection.Comparer);
            result.Remove(path);
            return result;
        }

        /// <summary>
        /// Removes multiple paths from the collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="paths">The paths to remove.</param>
        /// <returns>A new <see cref="FilePathCollection"/> that do not contain the provided paths.</returns>
        [SuppressMessage("Usage", "CA2225:Operator overloads have named alternates")]
        public static FilePathCollection operator -(FilePathCollection collection, IEnumerable<FilePath> paths)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            var result = new FilePathCollection(collection, collection.Comparer);
            result.Remove(paths);
            return result;
        }

        /// <inheritdoc/>
        public IEnumerator<FilePath> GetEnumerator()
        {
            return _paths.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
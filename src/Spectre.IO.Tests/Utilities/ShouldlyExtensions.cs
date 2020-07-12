using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Shouldly;

namespace Spectre.IO.Tests
{
    public static class ShouldlyExtensions
    {
        private static readonly PathComparer _comparer = new PathComparer(false);

        [SuppressMessage("Naming", "CA1720:Identifier contains type name")]
        public static T And<T>(this T obj)
        {
            return obj;
        }

        public static void ShouldContainFilePath(this IEnumerable<Path> paths, FilePath expected)
        {
            ShouldContainPath(paths, expected);
        }

        public static void ShouldContainDirectoryPath(this IEnumerable<Path> paths, DirectoryPath expected)
        {
            ShouldContainPath(paths, expected);
        }

        private static void ShouldContainPath<T>(this IEnumerable<Path> paths, T expected)
            where T : Path
        {
            // Find the path.
            var path = paths.FirstOrDefault(x => _comparer.Equals(x, expected));

            // Make sure that it is what we expect.
            path.ShouldNotBeNull();
            path.ShouldBeOfType<T>();
        }
    }
}

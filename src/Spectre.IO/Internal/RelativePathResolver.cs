﻿using System;
using System.Linq;

namespace Spectre.IO.Internal;

internal static class RelativePathResolver
{
    public static DirectoryPath Resolve(DirectoryPath from, DirectoryPath to)
    {
        if (from == null)
        {
            throw new ArgumentNullException(nameof(from));
        }

        if (to == null)
        {
            throw new ArgumentNullException(nameof(to));
        }

        if (to.IsRelative)
        {
            throw new InvalidOperationException("Target path must be an absolute path.");
        }

        if (from.IsRelative)
        {
            throw new InvalidOperationException("Source path must be an absolute path.");
        }

        if (from.Segments.Count == 0 && to.Segments.Count == 0)
        {
            return new DirectoryPath(".");
        }

        if (from.Segments[0] != to.Segments[0])
        {
            throw new InvalidOperationException("Paths must share a common prefix.");
        }

        if (string.CompareOrdinal(from.FullPath, to.FullPath) == 0)
        {
            return new DirectoryPath(".");
        }

        var minimumSegmentsLength = Math.Min(from.Segments.Count, to.Segments.Count);
        var numberOfSharedSegments = 1;

        for (var i = 1; i < minimumSegmentsLength; i++)
        {
            if (string.CompareOrdinal(from.Segments[i], to.Segments[i]) != 0)
            {
                break;
            }

            numberOfSharedSegments++;
        }

        var fromSegments = Enumerable.Repeat("..", from.Segments.Count - numberOfSharedSegments);
        var toSegments = to.Segments.Skip(numberOfSharedSegments);

        var relativePath = PathHelper.Combine(fromSegments.Concat(toSegments).ToArray());

        if (string.IsNullOrWhiteSpace(relativePath))
        {
            relativePath = ".";
        }

        return new DirectoryPath(relativePath);
    }
}
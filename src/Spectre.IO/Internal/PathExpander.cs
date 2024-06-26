﻿using System;
using System.Linq;

namespace Spectre.IO.Internal;

internal static class PathExpander
{
    public static string Expand(Path path, IEnvironment environment)
    {
        if (path.Segments.Count > 0)
        {
            if (path.Segments[0].Equals("~", StringComparison.OrdinalIgnoreCase))
            {
                var first = environment.HomeDirectory.FullPath;

                if (path.Segments.Count == 1)
                {
                    return first;
                }
                else
                {
                    var separator = path.Separator.ToString();
                    var second = string.Join(separator, path.Segments.Skip(1).Take(path.Segments.Count - 1));

                    return string.Concat(first, separator, second);
                }
            }
        }

        return path.FullPath;
    }
}
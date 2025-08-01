namespace Spectre.IO.Internal;

internal static class PathExpander
{
    public static string Expand(Path path, IEnvironment environment)
    {
        if (path.Segments.Count == 0 || !path.Segments[0].Equals("~", StringComparison.OrdinalIgnoreCase))
        {
            return path.FullPath;
        }

        var first = environment.HomeDirectory.FullPath;

        if (path.Segments.Count == 1)
        {
            return first;
        }

        var separator = path.Separator.ToString();
        var second = string.Join(separator, path.Segments.Skip(1).Take(path.Segments.Count - 1));

        return string.Concat(first, separator, second);
    }
}
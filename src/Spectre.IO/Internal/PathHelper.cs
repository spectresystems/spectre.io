namespace Spectre.IO.Internal;

internal static class PathHelper
{
    private const char Backslash = '\\';
    private const char Slash = '/';
    private const string UncPrefix = @"\\";

    public static string Combine(params string[] paths)
    {
        if (paths.Length == 0)
        {
            return string.Empty;
        }

        var current = paths[0];
        for (var index = 1; index < paths.Length; index++)
        {
            current = Combine(current, paths[index]);
        }

        return current;
    }

    public static string Combine(string first, string second)
    {
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);

        // Both empty?
        if (string.IsNullOrWhiteSpace(first) && string.IsNullOrWhiteSpace(second))
        {
            return string.Empty;
        }

        // First empty?
        if (string.IsNullOrWhiteSpace(first) && !string.IsNullOrWhiteSpace(second))
        {
            return second;
        }

        // Second empty?
        if (!string.IsNullOrWhiteSpace(first) && string.IsNullOrWhiteSpace(second))
        {
            return first;
        }

        var isUnc = first.StartsWith(UncPrefix, StringComparison.OrdinalIgnoreCase);

        // Trim separators.
        first = first.TrimEnd(Backslash, Slash);
        second = second.TrimStart(Backslash, Slash).TrimEnd(Backslash, Slash);

        // UNC root only?
        if (isUnc && string.IsNullOrWhiteSpace(first))
        {
            return string.Concat(UncPrefix, second);
        }

        var separator = isUnc ? Backslash : Slash;
        return string.Concat(first, separator, second);
    }

    public static bool HasExtension(FilePath path)
    {
        ArgumentNullException.ThrowIfNull(path);

        for (var index = path.FullPath.Length - 1; index >= 0; index--)
        {
            if (path.FullPath[index] == '.')
            {
                return true;
            }

            if (path.IsUNC && path.FullPath[index] == '\\')
            {
                break;
            }

            if (path.FullPath[index] == '/')
            {
                break;
            }
        }

        return false;
    }

    public static string GetDirectoryName(FilePath path)
    {
        ArgumentNullException.ThrowIfNull(path);

        switch (path.Segments.Count)
        {
            case 0:
                return string.Empty;
            case 1 when path.IsUNC:
                return @"\\";
            case 1 when path.Segments[0].Length >= 1 && path.Segments[0][0] == '/':
                return "/";
        }

        if (path.IsUNC)
        {
            var segments = path.Segments.Skip(1).Take(path.Segments.Count - 2);
            return @$"\\{string.Join("\\", segments)}";
        }

        return string.Join("/", path.Segments.Take(path.Segments.Count - 1));
    }

    public static string? GetFileName(FilePath path)
    {
        ArgumentNullException.ThrowIfNull(path);

        if (path.Segments.Count == 0)
        {
            return null;
        }

        var filename = path.Segments[^1];
        if (path.Segments.Count == 1 && !path.IsRelative)
        {
            if (path.Segments[0].StartsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                return filename.TrimStart('/');
            }

            return path.IsUNC
                ? filename.TrimStart('\\')
                : null;
        }

        return filename;
    }

    public static string? GetFileNameWithoutExtension(FilePath path)
    {
        var filename = GetFileName(path);
        if (filename == null)
        {
            return null;
        }

        var index = filename.LastIndexOf('.');
        return index != -1
            ? filename[..index]
            : filename;
    }

    public static string? ChangeExtension(FilePath path, string? extension)
    {
        ArgumentNullException.ThrowIfNull(path);

        if (extension == null)
        {
            return RemoveExtension(path);
        }

        if (string.IsNullOrWhiteSpace(extension))
        {
            // Empty extension is an extension consisting of only a period.
            extension = ".";
        }

        // Make sure that the extension has a dot.
        if (!extension.StartsWith(".", StringComparison.OrdinalIgnoreCase))
        {
            extension = $".{extension}";
        }

        // Empty path?
        var filename = path.FullPath;
        if (string.IsNullOrWhiteSpace(filename))
        {
            return null;
        }

        for (var index = path.FullPath.Length - 1; index >= 0; index--)
        {
            if (filename[index] == '/')
            {
                // No extension found.
                break;
            }

            if (filename[index] == '.')
            {
                // Replace the extension.
                return string.Concat(filename[..index], extension);
            }
        }

        return string.Concat(filename, extension);
    }

    public static string RemoveExtension(FilePath path)
    {
        ArgumentNullException.ThrowIfNull(path);

        for (var index = path.FullPath.Length - 1; index >= 0; index--)
        {
            if (path.FullPath[index] == '.')
            {
                return path.FullPath[..index];
            }

            if (path.IsUNC && path.FullPath[index] == '\\')
            {
                break;
            }

            if (path.FullPath[index] == '/')
            {
                break;
            }
        }

        return path.FullPath;
    }

    public static bool IsPathRooted(string path)
    {
        var length = path.Length;
        if (length >= 1)
        {
            // Root?
            if (path[0] == '/')
            {
                return true;
            }

            if (path.Length >= 2)
            {
                // UNC?
                if (path[0] == '\\' && path[1] == '\\')
                {
                    return true;
                }
            }

            // Windows drive?
            if (length >= 2 && path[1] == ':')
            {
                return true;
            }
        }

        return false;
    }
}
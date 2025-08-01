namespace Spectre.IO.Internal;

internal static class PathCollapser
{
    public static string Collapse(Path path)
    {
        ArgumentNullException.ThrowIfNull(path);

        var stack = new Stack<string>();
        foreach (var segment in path.FullPath.Split('/', '\\'))
        {
            if (segment == ".")
            {
                continue;
            }

            if (segment == "..")
            {
                if (stack.Count > 1)
                {
                    stack.Pop();
                }

                continue;
            }

            stack.Push(segment);
        }

        var separator = path.Separator.ToString();
        var collapsed = string.Join(separator, stack.Reverse());
        return string.IsNullOrEmpty(collapsed) ? "." : collapsed;
    }
}
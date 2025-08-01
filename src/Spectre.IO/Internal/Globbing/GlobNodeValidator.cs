namespace Spectre.IO.Internal;

internal static class GlobNodeValidator
{
    public static void Validate(string pattern, GlobNode node)
    {
        GlobNode? previous = null;
        var current = node;
        while (current != null)
        {
            if (previous is RecursiveWildcardNode)
            {
                if (current is ParentDirectoryNode)
                {
                    throw new NotSupportedException("Visiting a parent that is a recursive wildcard is not supported.");
                }
            }

            if (current is UncRootNode unc)
            {
                if (string.IsNullOrWhiteSpace(unc.Server))
                {
                    throw new IOException($"The pattern '{pattern}' has no server part specified.");
                }
            }

            previous = current;
            current = current.Next;
        }
    }
}
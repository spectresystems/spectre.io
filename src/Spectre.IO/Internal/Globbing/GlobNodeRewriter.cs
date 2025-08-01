namespace Spectre.IO.Internal;

internal static class GlobNodeRewriter
{
    public static GlobNode Rewrite(string pattern, IEnumerable<GlobNode> nodes)
    {
        return RewriteUncRoot(pattern, CreateLinkedList(
            RewriteSingleWildcards(nodes)));
    }

    private static GlobNode RewriteUncRoot(string pattern, GlobNode root)
    {
        if (root is UncRootNode unc && string.IsNullOrWhiteSpace(unc.Server))
        {
            var next = unc.Next;
            if (next == null)
            {
                throw new IOException($"The pattern '{pattern}' has no server part specified.");
            }
            else if (next is PathNode path && path.IsIdentifier)
            {
                // Rewrite the root node.
                return new UncRootNode(path.GetPath())
                {
                    Next = next.Next,
                };
            }
            else
            {
                throw new IOException($"The pattern '{pattern}' has an invalid server part specified.");
            }
        }

        return root;
    }

    private static GlobNode CreateLinkedList(IEnumerable<GlobNode> nodes)
    {
        var result = new Stack<GlobNode>();
        var stack = new Stack<GlobNode>(nodes);
        var previous = stack.Pop();
        result.Push(previous);
        while (stack.Count > 0)
        {
            var current = stack.Pop();
            current.Next = previous;
            previous = current;
            result.Push(previous);
        }

        return result.Pop();
    }

    private static IEnumerable<GlobNode> RewriteSingleWildcards(IEnumerable<GlobNode> nodes)
    {
        foreach (var node in nodes)
        {
            var segmentNode = node as PathNode;
            if (segmentNode?.Segments is [WildcardSegment])
            {
                yield return new WildcardNode();
                continue;
            }

            yield return node;
        }
    }
}
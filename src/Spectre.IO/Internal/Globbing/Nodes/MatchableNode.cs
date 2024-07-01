namespace Spectre.IO.Internal;

internal abstract class MatchableNode : GlobNode
{
    public abstract bool IsMatch(string value);
}
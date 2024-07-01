using System.Diagnostics;

namespace Spectre.IO.Internal;

[DebuggerDisplay("*")]
internal sealed class WildcardNode : MatchableNode
{
    [DebuggerStepThrough]
    public override void Accept(GlobVisitor visitor, GlobVisitorContext context)
    {
        visitor.VisitWildcardSegmentNode(this, context);
    }

    public override bool IsMatch(string value)
    {
        return true;
    }
}
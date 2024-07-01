using System.Diagnostics;

namespace Spectre.IO.Internal;

[DebuggerDisplay(".")]
internal sealed class CurrentDirectoryNode : GlobNode
{
    [DebuggerStepThrough]
    public override void Accept(GlobVisitor visitor, GlobVisitorContext context)
    {
        visitor.VisitCurrent(this, context);
    }
}
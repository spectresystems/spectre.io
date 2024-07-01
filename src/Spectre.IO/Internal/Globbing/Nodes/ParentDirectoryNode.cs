using System.Diagnostics;

namespace Spectre.IO.Internal;

[DebuggerDisplay("..")]
internal sealed class ParentDirectoryNode : GlobNode
{
    [DebuggerStepThrough]
    public override void Accept(GlobVisitor visitor, GlobVisitorContext context)
    {
        visitor.VisitParent(this, context);
    }
}
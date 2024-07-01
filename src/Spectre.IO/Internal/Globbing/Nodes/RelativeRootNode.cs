using System.Diagnostics;

namespace Spectre.IO.Internal;

[DebuggerDisplay("./")]
internal sealed class RelativeRootNode : GlobNode
{
    [DebuggerStepThrough]
    public override void Accept(GlobVisitor globber, GlobVisitorContext context)
    {
        globber.VisitRelativeRoot(this, context);
    }
}
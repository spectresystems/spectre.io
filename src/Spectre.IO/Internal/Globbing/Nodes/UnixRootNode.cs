using System.Diagnostics;

namespace Spectre.IO.Internal
{
    [DebuggerDisplay("/")]
    internal sealed class UnixRootNode : GlobNode
    {
        [DebuggerStepThrough]
        public override void Accept(GlobVisitor visitor, GlobVisitorContext context)
        {
            visitor.VisitUnixRoot(this, context);
        }
    }
}
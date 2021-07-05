using System.Diagnostics;

namespace Spectre.IO.Internal
{
    [DebuggerDisplay("~")]
    internal sealed class HomeDirectoryNode : GlobNode
    {
        [DebuggerStepThrough]
        public override void Accept(GlobVisitor visitor, GlobVisitorContext context)
        {
            visitor.VisitHomeDirectory(this, context);
        }
    }
}
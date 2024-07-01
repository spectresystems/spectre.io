using System.Diagnostics;

namespace Spectre.IO.Internal;

[DebuggerDisplay(@"\\")]
internal sealed class UncRootNode : GlobNode
{
    public string Server { get; }

    public UncRootNode(string server)
    {
        Server = server;
    }

    [DebuggerStepThrough]
    public override void Accept(GlobVisitor visitor, GlobVisitorContext context)
    {
        visitor.VisitUncRoot(this, context);
    }
}
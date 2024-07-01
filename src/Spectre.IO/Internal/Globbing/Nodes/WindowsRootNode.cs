using System;
using System.Diagnostics;

namespace Spectre.IO.Internal;

[DebuggerDisplay("{Drive,nq}:")]
internal sealed class WindowsRootNode : GlobNode
{
    public string Drive { get; }

    public WindowsRootNode(string drive)
    {
        Drive = drive ?? throw new ArgumentNullException(nameof(drive));
    }

    [DebuggerStepThrough]
    public override void Accept(GlobVisitor visitor, GlobVisitorContext context)
    {
        visitor.VisitWindowsRoot(this, context);
    }
}
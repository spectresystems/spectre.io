using System.Diagnostics;

namespace Spectre.IO.Internal
{
    [DebuggerDisplay("**")]
    internal sealed class RecursiveWildcardNode : MatchableNode
    {
        [DebuggerStepThrough]
        public override void Accept(GlobVisitor globber, GlobVisitorContext context)
        {
            globber.VisitRecursiveWildcardSegment(this, context);
        }

        public override bool IsMatch(string value)
        {
            return true;
        }
    }
}
namespace Spectre.IO.Internal
{
    internal sealed class WildcardSegment : PathSegment
    {
        public override string Value => "*";

        public override string Regex => ".*";
    }
}

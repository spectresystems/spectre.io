namespace Spectre.IO.Internal
{
    internal sealed class BraceExpansionSegment : PathSegment
    {
        public override string Value { get; }

        public override string Regex { get; }

        public BraceExpansionSegment(string value)
        {
            Value = $"{{{value}}}";
            Regex = $"({value})".Replace(",", "|");
        }
    }
}

namespace Spectre.IO.Internal
{
    internal sealed class CharacterWildcardSegment : PathSegment
    {
        public override string Value => "?";

        public override string Regex => ".{1}";
    }
}

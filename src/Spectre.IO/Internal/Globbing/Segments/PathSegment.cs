namespace Spectre.IO.Internal;

internal abstract class PathSegment
{
    public abstract string Regex { get; }
    public abstract string Value { get; }
}
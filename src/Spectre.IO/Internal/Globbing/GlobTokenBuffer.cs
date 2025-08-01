namespace Spectre.IO.Internal;

internal sealed class GlobTokenBuffer
{
    private readonly Queue<GlobToken> _tokens;

    public int Count => _tokens.Count;

    public GlobTokenBuffer(IEnumerable<GlobToken> tokens)
    {
        _tokens = new Queue<GlobToken>(tokens);
    }

    public GlobToken? Peek()
    {
        return _tokens.Count == 0
            ? null
            : _tokens.Peek();
    }

    public GlobToken? Read()
    {
        return _tokens.Count == 0
            ? null
            : _tokens.Dequeue();
    }
}
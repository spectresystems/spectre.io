using System.Text.RegularExpressions;

namespace Spectre.IO.Internal;

internal sealed class GlobParserContext
{
    private readonly GlobTokenBuffer _buffer;

    public string Pattern { get; }
    public int TokenCount => _buffer.Count;
    public GlobToken? CurrentToken { get; private set; }
    public RegexOptions Options { get; }

    public GlobParserContext(string pattern, IPathComparer comparer)
    {
        _buffer = GlobTokenizer.Tokenize(pattern);

        Pattern = pattern;
        CurrentToken = null;
        Options = RegexOptions.Compiled | RegexOptions.Singleline;

        if (!comparer.IsCaseSensitive)
        {
            Options |= RegexOptions.IgnoreCase;
        }
    }

    public GlobToken? Peek()
    {
        return _buffer.Peek();
    }

    public GlobToken? Accept()
    {
        var result = CurrentToken;
        CurrentToken = _buffer.Read();
        return result;
    }

    public GlobToken Accept(params GlobTokenKind[] kind)
    {
        if (CurrentToken == null)
        {
            throw new InvalidOperationException("Unable to accept glob token.");
        }

        if (kind.Any(k => k == CurrentToken.Kind))
        {
            var result = CurrentToken;
            Accept();
            return result;
        }

        throw new InvalidOperationException("Unexpected glob token kind.");
    }
}
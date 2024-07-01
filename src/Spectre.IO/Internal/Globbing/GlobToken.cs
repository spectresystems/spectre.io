using System.Diagnostics;

namespace Spectre.IO.Internal;

[DebuggerDisplay("{Value,nq} ({Kind,nq})")]
internal sealed class GlobToken
{
    public GlobTokenKind Kind { get; }

    public string Value { get; }

    public GlobToken(GlobTokenKind kind, string value)
    {
        Kind = kind;
        Value = value;
    }
}
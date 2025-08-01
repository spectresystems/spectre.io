﻿using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Spectre.IO.Internal;

[DebuggerDisplay("{GetPath(),nq}")]
internal sealed class PathNode : MatchableNode
{
    private readonly Regex _regex;

    public IReadOnlyList<PathSegment> Segments { get; }
    public bool IsIdentifier { get; }

    public PathNode(List<PathSegment> tokens, RegexOptions options)
    {
        _regex = CreateRegex(tokens, options);

        Segments = tokens;
        IsIdentifier = Segments is [TextSegment];
    }

    public override bool IsMatch(string value)
    {
        return _regex.IsMatch(value);
    }

    public string GetPath()
    {
        var builder = new StringBuilder();
        foreach (var token in Segments)
        {
            builder.Append(token.Value);
        }

        return builder.ToString();
    }

    [DebuggerStepThrough]
    public override void Accept(GlobVisitor globber, GlobVisitorContext context)
    {
        globber.VisitSegment(this, context);
    }

    private static Regex CreateRegex(List<PathSegment> tokens, RegexOptions options)
    {
        var builder = new StringBuilder();
        foreach (var token in tokens)
        {
            builder.Append(token.Regex);
        }

        return new Regex(string.Concat("^", builder.ToString(), "$"), options);
    }
}
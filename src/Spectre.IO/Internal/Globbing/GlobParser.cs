using System;
using System.Collections.Generic;
using System.Linq;

namespace Spectre.IO.Internal
{
    internal sealed class GlobParser
    {
        private readonly IEnvironment _environment;

        public GlobParser(IEnvironment environment)
        {
            _environment = environment;
        }

        public GlobNode Parse(string pattern, PathComparer comparer)
        {
            return Parse(new GlobParserContext(pattern, comparer));
        }

        private GlobNode Parse(GlobParserContext context)
        {
            context.Accept();

            // Parse the root.
            var items = new List<GlobNode> { ParseRoot(context) };
            if (items.Count == 1 && items[0] is RelativeRootNode)
            {
                items.Add(ParseNode(context));
            }

            // Parse all path segments.
            while (context.TokenCount > 0 && context.CurrentToken?.Kind == GlobTokenKind.PathSeparator)
            {
                context.Accept();
                items.Add(ParseNode(context));
            }

            // Rewrite the items into a linked list.
            var result = GlobNodeRewriter.Rewrite(context.Pattern, items);
            GlobNodeValidator.Validate(context.Pattern, result);
            return result;
        }

        private GlobNode ParseRoot(GlobParserContext context)
        {
            if (context.CurrentToken == null)
            {
                return new RelativeRootNode();
            }

            if (_environment.Platform.IsUnix())
            {
                // Starts with a separator?
                if (context.CurrentToken.Kind == GlobTokenKind.PathSeparator)
                {
                    return new UnixRootNode();
                }
            }
            else
            {
                // Starts with a separator?
                if (context.CurrentToken.Kind == GlobTokenKind.PathSeparator)
                {
                    if (context.Peek()?.Kind == GlobTokenKind.PathSeparator)
                    {
                        context.Accept();
                        return new UncRootNode(string.Empty);
                    }

                    // Get the drive from the working directory.
                    var workingDirectory = _environment.WorkingDirectory;
                    var root = workingDirectory.FullPath.Split(':').First();
                    return new WindowsRootNode(root);
                }

                // Is this a drive?
                if (context.CurrentToken.Kind == GlobTokenKind.Text &&
                    context.CurrentToken.Value.Length == 1 &&
                    context.Peek()?.Kind == GlobTokenKind.WindowsRoot)
                {
                    var identifier = ParseText(context);
                    context.Accept(GlobTokenKind.WindowsRoot);
                    return new WindowsRootNode(identifier.Value);
                }
            }

            // Starts with an identifier?
            if (context.CurrentToken.Kind == GlobTokenKind.Text)
            {
                // Is the identifier indicating a current directory?
                if (context.CurrentToken.Value == ".")
                {
                    context.Accept(GlobTokenKind.Text);
                    if (context.CurrentToken.Kind != GlobTokenKind.PathSeparator)
                    {
                        throw new InvalidOperationException();
                    }

                    context.Accept(GlobTokenKind.PathSeparator);
                }
            }

            return new RelativeRootNode();
        }

        private static GlobNode ParseNode(GlobParserContext context)
        {
            if (context.CurrentToken?.Kind == GlobTokenKind.Wildcard)
            {
                var next = context.Peek();
                if (next != null && next.Kind == GlobTokenKind.Wildcard)
                {
                    context.Accept(GlobTokenKind.Wildcard);
                    context.Accept(GlobTokenKind.Wildcard);
                    return new RecursiveWildcardNode();
                }
            }
            else if (context.CurrentToken?.Kind == GlobTokenKind.Parent)
            {
                context.Accept(GlobTokenKind.Parent);
                return new ParentDirectoryNode();
            }
            else if (context.CurrentToken?.Kind == GlobTokenKind.Current)
            {
                context.Accept(GlobTokenKind.Current);
                return new CurrentDirectoryNode();
            }

            var items = new List<PathSegment>();
            while (true)
            {
                switch (context.CurrentToken?.Kind)
                {
                    case GlobTokenKind.Text:
                    case GlobTokenKind.CharacterWildcard:
                    case GlobTokenKind.Wildcard:
                    case GlobTokenKind.BracketWildcard:
                    case GlobTokenKind.BraceExpansion:
                        items.Add(ParsePathSegment(context));
                        continue;
                }

                break;
            }

            return new PathNode(items, context.Options);
        }

        private static PathSegment ParsePathSegment(GlobParserContext context)
        {
            if (context.CurrentToken != null)
            {
                switch (context.CurrentToken.Kind)
                {
                    case GlobTokenKind.Text:
                        return ParseText(context);
                    case GlobTokenKind.CharacterWildcard:
                    case GlobTokenKind.Wildcard:
                        return ParseWildcard(context);
                    case GlobTokenKind.BracketWildcard:
                        return ParseBracketWildcard(context);
                    case GlobTokenKind.BraceExpansion:
                        return ParseBraceExpansion(context);
                }
            }

            throw new NotSupportedException("Unable to parse sub segment.");
        }

        private static PathSegment ParseText(GlobParserContext context)
        {
            var token = context.Accept(GlobTokenKind.Text);
            return new TextSegment(token.Value);
        }

        private static PathSegment ParseWildcard(GlobParserContext context)
        {
            var token = context.Accept(GlobTokenKind.Wildcard, GlobTokenKind.CharacterWildcard);
            return token.Kind == GlobTokenKind.Wildcard
                ? (PathSegment)new WildcardSegment()
                : new CharacterWildcardSegment();
        }

        private static PathSegment ParseBracketWildcard(GlobParserContext context)
        {
            var token = context.Accept(GlobTokenKind.BracketWildcard);
            return new BracketWildcardSegment(token.Value);
        }

        private static BraceExpansionSegment ParseBraceExpansion(GlobParserContext context)
        {
            var token = context.Accept(GlobTokenKind.BraceExpansion);
            return new BraceExpansionSegment(token.Value);
        }
    }
}
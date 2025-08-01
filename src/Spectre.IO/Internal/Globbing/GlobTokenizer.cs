using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Spectre.IO.Internal;

internal sealed class GlobTokenizer
{
    public static GlobTokenBuffer Tokenize(string input)
    {
        using (var reader = new StringReader(input))
        {
            return Tokenize(reader);
        }
    }

    private static GlobTokenBuffer Tokenize(StringReader reader)
    {
        var tokens = new List<GlobToken>();
        while (reader.Peek() != -1)
        {
            var token = ReadToken(reader);
            tokens.Add(token);
        }

        var queue = new Queue<GlobToken>(tokens);
        var result = new List<GlobToken>();
        while (queue.Count > 0)
        {
            var current = queue.Peek();
            if (current.Kind == GlobTokenKind.Text)
            {
                var accumulator = new List<GlobToken>();
                while (queue.Count > 0)
                {
                    var item = queue.Peek();
                    if (item.Kind != GlobTokenKind.Text)
                    {
                        break;
                    }

                    accumulator.Add(queue.Dequeue());
                }

                result.Add(
                    new GlobToken(
                        GlobTokenKind.Text,
                        string.Concat(accumulator.Select(i => i.Value))));
            }
            else
            {
                result.Add(queue.Dequeue());
            }
        }

        // Turn the tokens into a token buffer.
        return new GlobTokenBuffer(result);
    }

    private static GlobToken ReadToken(StringReader reader)
    {
        var current = (char)reader.Peek();

        if (current == '?')
        {
            reader.Read();
            return new GlobToken(GlobTokenKind.CharacterWildcard, "?");
        }
        else if (current == '~')
        {
            reader.Read();
            return new GlobToken(GlobTokenKind.HomeDirectory, "~");
        }
        else if (current == '*')
        {
            reader.Read();
            return new GlobToken(GlobTokenKind.Wildcard, "*");
        }
        else if (current == '.')
        {
            reader.Read();
            if (reader.Peek() != -1)
            {
                var next = (char)reader.Peek();
                if (next is '/' or '\\')
                {
                    return new GlobToken(GlobTokenKind.Current, ".");
                }

                if (next == '.')
                {
                    reader.Read();
                    return new GlobToken(GlobTokenKind.Parent, "..");
                }
            }

            return new GlobToken(GlobTokenKind.Text, current.ToString(CultureInfo.InvariantCulture));
        }
        else if (current == ':')
        {
            reader.Read();
            return new GlobToken(GlobTokenKind.WindowsRoot, ":");
        }
        else if (current == '[')
        {
            return ReadScope(reader, GlobTokenKind.BracketWildcard, '[', ']');
        }
        else if (current == '{')
        {
            return ReadScope(reader, GlobTokenKind.BraceExpansion, '{', '}');
        }
        else if (current == '\\' || current == '/')
        {
            reader.Read();
            return new GlobToken(GlobTokenKind.PathSeparator, "/");
        }

        reader.Read();
        return new GlobToken(GlobTokenKind.Text, current.ToString(CultureInfo.InvariantCulture));
    }

    private static GlobToken ReadScope(StringReader reader, GlobTokenKind kind, char first, char last)
    {
        var current = (char)reader.Read();
        Debug.Assert(current == first, "Unexpected token.");

        var accumulator = new StringBuilder();
        while (reader.Peek() != -1)
        {
            current = (char)reader.Peek();
            if (current == last)
            {
                break;
            }

            accumulator.Append((char)reader.Read());
        }

        if (reader.Peek() == -1)
        {
            throw new InvalidOperationException($"Expected '{last}' but reached end of pattern.");
        }

        // Consume the last character.
        current = (char)reader.Read();
        Debug.Assert(current == last, "Unexpected token.");

        return new GlobToken(kind, accumulator.ToString());
    }
}
using System;
using System.Text;

internal sealed class IndentedStringBuilder
{
    private readonly StringBuilder _builder;
    private int _indentation;

    public IndentedStringBuilder()
    {
        _builder = new StringBuilder();
    }

    private class IndentationScope : IDisposable
    {
        private readonly Action _action;

        public IndentationScope(Action action)
        {
            _action = action;
        }

        ~IndentationScope()
        {
            throw new InvalidOperationException("Indentation not closed");
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _action();
        }
    }

    public IDisposable Indent()
    {
        _indentation += 1;
        return new IndentationScope(() => _indentation--);
    }

    public IDisposable Outdent()
    {
        _indentation -= 1;
        return new IndentationScope(() => _indentation++);
    }

    public void Append(string text)
    {
        _builder.Append(text);
    }

    public void AppendLine(string text)
    {
        _builder.Append(new string(' ', _indentation * 4));
        _builder.AppendLine(text);
    }

    public void AppendLine()
    {
        _builder.AppendLine();
    }

    public override string ToString()
    {
        return _builder.ToString();
    }
}
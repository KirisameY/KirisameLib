using System;
using System.Linq;
using System.Text;

namespace KirisameLib.GeneratorTools;

public class IndentStringBuilder(string indentContent)
{
    public IndentStringBuilder(int indentLength = 4) : this(new string(' ', indentLength)) { }

    private readonly StringBuilder _builder = new();
    private int _currentLevel;
    private string _current = string.Empty;

    private bool _currentLineIntended = false;

    private void UpdateCurrent() =>
        _current = string.Concat(Enumerable.Repeat(indentContent, _currentLevel));

    public IndentStringBuilder IncreaseIndent()
    {
        _currentLevel++;
        UpdateCurrent();
        return this;
    }

    public IndentStringBuilder DecreaseIndent()
    {
        _currentLevel--;
        UpdateCurrent();
        return this;
    }

    public readonly struct IndentDisposable(IndentStringBuilder builder) : IDisposable
    {
        public void Dispose()
        {
            builder.DecreaseIndent();
        }
    }

    public IndentDisposable Indent()
    {
        IncreaseIndent();
        return new(this);
    }

    public IndentStringBuilder Append(string content)
    {
        if (!_currentLineIntended)
        {
            _builder.Append(_current);
            _currentLineIntended = true;
        }
        _builder.Append(content);
        return this;
    }

    public IndentStringBuilder AppendLine(string content)
    {
        if (!_currentLineIntended)
            _builder.Append(_current);
        _builder.AppendLine(content);
        _currentLineIntended = false;
        return this;
    }

    public IndentStringBuilder AppendLine() => AppendLine(string.Empty);

    public override string ToString() => _builder.ToString();
}
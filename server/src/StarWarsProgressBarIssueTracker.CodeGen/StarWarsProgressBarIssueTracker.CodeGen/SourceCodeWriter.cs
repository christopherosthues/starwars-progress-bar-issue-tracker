using System.Text;

namespace StarWarsProgressBarIssueTracker.CodeGen;

public class SourceCodeWriter
{
    private int _tabLevel;
    private readonly StringBuilder _builder = new();

    public void WriteLine()
    {
        _builder.AppendLine();
    }

    private void WriteTabs()
    {
        for (int i = 0; i < _tabLevel; i++)
        {
            _builder.Append("    ");
        }
    }

    public void WriteLine(string line)
    {
        if (string.IsNullOrEmpty(line))
        {
            return;
        }

        if (line[0].Equals('}'))
        {
            _tabLevel--;
        }

        WriteTabs();

        _builder.AppendLine(line);

        if (line[0].Equals('{'))
        {
            _tabLevel++;
        }
    }

    public override string ToString() => _builder.ToString();
}

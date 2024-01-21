namespace MergeSharpSource;

internal class CodeJournal
{
    private readonly List<string> _usings = [];
    private readonly List<string> _codes = [];

    public IEnumerable<string> GetWarningConfig()
    {
        return ["#pragma warning disable"]; // all disable
    }
    public IEnumerable<string> GetUsings()
        => _usings
        .ToHashSet()
        .Order()
        ;
    public IEnumerable<string> GetCodes()
        => _codes;

    private List<string>? SelectTarget(string line)
    {
        if (line.StartsWith("using"))
        {
            return _usings;
        }
        if (line.StartsWith("#pragma warning"))
        {
            return null; // discard
        }
        return _codes;
    }
    public void Add(string line) => SelectTarget(line)?.Add(line);
}

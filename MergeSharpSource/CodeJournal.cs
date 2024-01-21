namespace MergeSharpSource;

internal class CodeJournal
{
    private readonly List<string> _usings = [];
    private readonly List<string> _codes = [];

    public IEnumerable<string> GetUsings()
        => _usings
        .ToHashSet()
        .Order()
        ;
    public IEnumerable<string> GetCodes()
        => _codes;
    public void Add(string line)
    {
        (line.StartsWith("using") ? _usings : _codes)
            .Add(line);
    }
}

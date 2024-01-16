namespace MergeSharpSource;

internal class Options
{
    public string[] Exclude { get; set; } = [];
    [SharpArgParse.Alias('r')]
    public bool Recursive { get; set; }
    [SharpArgParse.Alias('o')]
    public string Output { get; set; } = "-";

}

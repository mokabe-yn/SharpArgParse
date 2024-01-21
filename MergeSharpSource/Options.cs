namespace MergeSharpSource;

internal class Options
{
    // for license or readme
    public string[] EmbedText { get; set; } = [];
    public string[] Exclude { get; set; } = [];
    [SharpArgParse.Alias('r')]
    public bool Recursive { get; set; }
    [SharpArgParse.Alias('o')]
    public string Output { get; set; } = "-";

}

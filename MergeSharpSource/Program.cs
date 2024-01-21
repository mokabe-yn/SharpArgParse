namespace MergeSharpSource;

internal class Program
{
    static int Main(string[] args)
    {
        var (opts, targets) = SharpArgParse.ArgParse<Options>.Parse(args);
        string[] files = FileNameNormalize.Normalize(targets, opts);

        CodeJournal cj = new();
        foreach (string path in files)
        {
            using StreamReader sr = new(path);
            string? line;
            while ((line = sr.ReadLine()) is not null)
            {
                cj.Add(line);
            }
        }


        // output
        using TextWriter? sw = opts.Output != "-" ? new StreamWriter(opts.Output) : null;
        var dst = sw ?? Console.Out;

        IEnumerable<string>[] outputs = [
            cj.GetWarningConfig(),
            cj.GetUsings(),
            cj.GetCodes(),
        ];
        foreach (string line in outputs.Chain())
        {
            dst.WriteLine(line);
        }
        return 0;
    }

}

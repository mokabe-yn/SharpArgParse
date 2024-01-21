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
            using var sr = path != "-" ? new StreamReader(path) : null;
            foreach(string line in FileReadWrite.ReadLines(sr ?? Console.In))
            {
                cj.Add(line);
            }
        }


        // output
        using var sw = opts.Output != "-" ? new StreamWriter(opts.Output) : null;
        var dst = sw ?? Console.Out;

        foreach (var path in opts.EmbedText)
        {
            using var sr = path != "-" ? new StreamReader(path) : null;
            foreach (string line in FileReadWrite.ReadLines(sr ?? Console.In))
            {
                dst.WriteLine("// " + line);
            }
        }

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

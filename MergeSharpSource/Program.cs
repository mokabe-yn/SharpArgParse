namespace MergeSharpSource;

internal class Program
{
    static int Main(string[] args)
    {
        var (opts, targets) = SharpArgParse.ArgParse<Options>.Parse(args);
        string[] files = FileNameNormalize.Normalize(targets, opts);

        CodeJournal cj = new();
        foreach (string path in targets)
        {
            using StreamReader sr = new(path);
            string? line;
            while ((line = sr.ReadLine()) is not null)
            {
                cj.Add(line);
            }
        }

        IEnumerable<string>[] outputs = [
            cj.GetUsings(),
            cj.GetCodes(),
        ];
        foreach (string line in outputs.Chain())
        {
            Console.WriteLine(line);
        }
        return 0;
    }

}


namespace MergeSharpSource;

internal class Program
{
    static int Main(string[] args)
    {
        var (opts, targets) = SharpArgParse.ArgParse<Options>.Parse(args);
        if (opts.Recursive)
        {
            targets = targets
                .Select(t => Recursive(t, opts.Exclude))
                .Chain()
                .Where(p => p.EndsWith(".cs"))
                .ToArray();
        }
        List<string> usings = [];
        List<string> codes = [];
        foreach (string path in targets)
        {
            using StreamReader sr = new(path);
            string? line;
            while ((line = sr.ReadLine()) is not null)
            {
                (line.StartsWith("using") ? usings : codes)
                    .Add(line);
            }
        }

        foreach (string line in usings.ToHashSet().Order())
        {
            Console.WriteLine(line);
        }
        foreach (string line in codes)
        {
            Console.WriteLine(line);
        }
        return 0;
    }

    static IEnumerable<string> RecursiveCore(string targetpath, string[] excludes)
    {
        foreach (var p in Directory.EnumerateFileSystemEntries(targetpath))
        {
            if (excludes.Contains(Path.GetFileName(p)))
            {
                continue;
            }

            // file type only
            if (File.Exists(p) && !Directory.Exists(p))
            {
                yield return p;
                continue;
            }
            // recursive
            foreach (var pp in RecursiveCore(p, excludes))
            {
                yield return pp;
            }
        }
    }
    static IEnumerable<string> Recursive(string targetpath, string[] excludes)
    {
        if (excludes.Contains(Path.GetFileName(targetpath))) return [];
        if (File.Exists(targetpath)) return [targetpath];

        return RecursiveCore(targetpath, excludes);
    }
}

file static class Ex
{
    public static IEnumerable<T> Chain<T>(this IEnumerable<IEnumerable<T>> source)
    {
        foreach(var x in source)
        {
            foreach(var y in x)
            {
                yield return y;
            }
        }
    }
}
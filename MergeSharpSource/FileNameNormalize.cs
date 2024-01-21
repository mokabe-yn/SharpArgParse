namespace MergeSharpSource;

internal static class FileNameNormalize
{
    private static IEnumerable<string> RecursiveCore(string targetpath, string[] excludes)
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
    public static IEnumerable<string> Recursive(string targetpath, string[] excludes)
    {
        if (excludes.Contains(Path.GetFileName(targetpath))) return [];
        if (File.Exists(targetpath)) return [targetpath];

        return RecursiveCore(targetpath, excludes);
    }
    public static string[] Normalize(string[] files, Options opts)
    {
        if (opts.Recursive)
        {
            return files
                .Select(f => Recursive(f, opts.Exclude))
                .Chain()
                .Where(p => p.EndsWith(".cs"))
                .ToArray();
        }
        else
        {
            return files;
        }
    }
}

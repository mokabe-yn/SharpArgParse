namespace MergeSharpSource;

internal static class ExtraLinq
{
    public static IEnumerable<T> Chain<T>(this IEnumerable<IEnumerable<T>> source)
    {
        foreach (var x in source)
        {
            foreach (var y in x)
            {
                yield return y;
            }
        }
    }
}

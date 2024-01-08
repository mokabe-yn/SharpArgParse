using System.Reflection;

namespace SharpArgParse;

/// <summary>
/// Parses command line option argument. 
/// Use <see cref="ArgParse{TOptions}.Parse(string[], bool)"/>
/// </summary>
/// <remarks>
/// <typeparamref name="TOptions"/> is target options class. e.g.
/// <code><![CDATA[
/// // prog.exe --target-file a.txt --export
/// class Options {
///     public string? TargetFile { get; set; }
///     public bool Export { get; set; }
/// }
/// ]]></code>
/// Use helper attributes: 
/// <see cref="AliasAttribute"/> and 
/// <see cref="ValueAliasAttribute"/>.
/// </remarks>
/// <typeparam name="TOptions">Target option type</typeparam>
#if ARGPARCE_EXPORT
public
#else
internal
#endif
static class ArgParse<TOptions> where TOptions : new()
{
    private static System.Reflection.PropertyInfo[] GetPropertiesInfo()
    {
        return typeof(TOptions).GetProperties(
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.SetProperty |
            System.Reflection.BindingFlags.GetProperty |
            0);
    }

    private static IEnumerable<Trigger> ToTrigger(PropertyInfo prop)
    {
        yield return new Trigger(prop);
        foreach (var attr in (AliasAttribute[])prop.GetCustomAttributes(typeof(AliasAttribute), true))
        {
            yield return new Trigger(prop, attr);
        }
    }

    /// <summary>
    /// Parses command line option argument. 
    /// </summary>
    /// <param name="args">command line arguments</param>
    /// <param name="allowLater">
    /// allow later option then argument.
    /// e.g. <c><![CDATA[grep PATTERN -E]]></c>
    /// </param>
    /// <returns>setted options and rest arguments</returns>
    /// <exception cref="CommandLineException">invalid command line</exception>
    /// <exception cref="SettingMisstakeException" />
    public static ParseResult<TOptions> Parse(
        string[] args, bool allowLater = true)
    {
        var infos = GetPropertiesInfo();
        Trigger[] triggers = Utility.Chain(infos.Select(ToTrigger)).ToArray();

        // TODO: validate no dup.
        Trigger[] shorts = triggers.Where(t => t.IsShortTrigger).ToArray();
        Trigger[] longs = triggers.Where(t => !t.IsShortTrigger).ToArray();

        var m = new ArgumentReciever<TOptions>(
            new TOptions(), allowLater, shorts, longs);
        foreach (string arg in args)
        {
            m.Next(arg);
        }
        m.Validate();
        return new ParseResult<TOptions>(m.GetOptions(), m.GetRest());
    }
}
/// <summary>
/// Parses command line option argument. 
/// Use <see cref="Parse(string[], bool)"/>
/// </summary>
/// <inheritdoc cref="ArgParse{TOptions}"/>
#if ARGPARCE_EXPORT
public
#else
internal
#endif
static class ArgParse
{
    // alias ArgParse<TOptions>
    /// <inheritdoc cref="ArgParse{TOptions}.Parse(string[], bool)"/>
    /// <typeparam name="TOptions">
    /// <inheritdoc cref="ArgParse{TOptions}"/>
    /// </typeparam>
    public static ParseResult<TOptions> Parse<TOptions>(
        string[] args, bool allowLater = true)
        where TOptions : new() =>
        ArgParse<TOptions>.Parse(args, allowLater);
}

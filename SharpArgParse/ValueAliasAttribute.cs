namespace SharpArgParse;

/// <summary>
/// specify value to target property
/// </summary>
/// <remarks>
/// e.g. <code><![CDATA[
/// class Options {
///     // prog.exe --mode sorting-network --net -n
///     [ValueAlias("net", SortMode.SortingNetwork)]
///     [ValueAlias('n', SortMode.SortingNetwork)]
///     public SortMode Mode { get; set; }
///     enum SortMode {
///         Bubble,
///         Stooge,
///         Bogo,
///         SortingNetwork,
///     }
/// }
/// ]]></code>
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
#if ARGPARCE_EXPORT
public
#else
internal
#endif
class ValueAliasAttribute : AliasAttribute
{
    /// <summary/>
    public object Value { get; }
    /// <inheritdoc cref="AliasAttribute(string)"/>
    /// <param name="value">setting value</param>
    public ValueAliasAttribute(string alias, object value)
        : base(alias)
    {
        if (value is null) throw new ArgumentNullException(nameof(value));
        Value = value;
    }
    /// <inheritdoc cref="AliasAttribute(char)"/>
    /// <inheritdoc cref="ValueAliasAttribute(string, object)"/>
    public ValueAliasAttribute(char shortAlias, object value)
        : base(shortAlias)
    {
        if (value is null) throw new ArgumentNullException(nameof(value));
        Value = value;
    }
}

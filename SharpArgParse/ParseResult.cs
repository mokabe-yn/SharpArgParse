namespace SharpArgParse
{

    /// <summary>
    /// Parsed results Tuple. 
    /// </summary>
    /// <typeparam name="TOptions">
    /// <inheritdoc cref="ArgParse{TOptions}"/>
    /// </typeparam>
#if ARGPARCE_EXPORT
    public
#else
    internal
#endif
    readonly struct ParseResult<TOptions>
    {
        /// <summary>result</summary>
        public TOptions Options { get; }
        /// <summary>rest arguments</summary>
        public string[] RestArgs { get; }
        /// <summary/>
        public ParseResult(TOptions options, string[] restArgs)
        {
            Options = options;
            RestArgs = restArgs;
        }
        /// <summary>for <c>var (opts, rest) = ...;</c></summary>
        public void Deconstruct(out TOptions options, out string[] restArgs)
        {
            options = Options;
            restArgs = RestArgs;
        }
    }
}

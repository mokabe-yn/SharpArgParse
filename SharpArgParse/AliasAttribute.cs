using System;

namespace SharpArgParse
{
    /// <summary>
    /// sets option alias
    /// </summary>
    /// <remarks>
    /// e.g. <code><![CDATA[
    /// class Options {
    ///     // prog.exe --alias-style -B -c
    ///     [Alias("alias-style")]
    ///     [Alias('B')]
    ///     [Alias('c')]
    ///     public bool Flag { get; set; }
    /// }
    /// ]]></code>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
#if ARGPARCE_EXPORT
    public
#else
    internal
#endif
    class AliasAttribute : Attribute
    {
        /// <summary>long option style alias</summary>
        /// <remarks>e.g. <c>"another-style"</c></remarks>
        public string Alias { get; }
        /// <summary>short option style alias</summary>
        public char ShortAlias { get; }
        /// <summary>alias mode is short</summary>
        public bool IsShortAlias { get; }

        /// <param name="alias">long option style alias</param>
        public AliasAttribute(string alias)
        {
            if (alias is null) throw new ArgumentNullException(nameof(alias));
            Alias = alias;
            ShortAlias = '\0';
            IsShortAlias = false;
        }
        /// <param name="shortAlias">short option style alias</param>
        public AliasAttribute(char shortAlias)
        {
            Alias = "";
            ShortAlias = shortAlias;
            IsShortAlias = true;
        }
    }
}

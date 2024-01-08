using System;

namespace SharpArgParse
{
    /// <summary>this librarys internal error</summary>
    internal class InternalException : Exception
    {
        /// <inheritdoc cref="Exception(string)"/>
        internal InternalException(string message) : base(message) { }
        /// <inheritdoc cref="Exception(string, Exception)"/>
        internal InternalException(string message, Exception innerException)
            : base(message, innerException) { }
    }
    /// <summary>using this library incorrectly</summary>
#if ARGPARCE_EXPORT
    public
#else
    internal
#endif
    class SettingMisstakeException : Exception
    {
        /// <inheritdoc cref="Exception(string)"/>
        internal SettingMisstakeException(string message) : base(message) { }
        /// <inheritdoc cref="Exception(string, Exception)"/>
        internal SettingMisstakeException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    /// <summary>invalid commandline</summary>
#if ARGPARCE_EXPORT
    public
#else
    internal
#endif
    class CommandLineException : Exception
    {
        /// <inheritdoc cref="Exception(string)"/>
        internal CommandLineException(string message) : base(message) { }
        /// <inheritdoc cref="Exception(string, Exception)"/>
        internal CommandLineException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
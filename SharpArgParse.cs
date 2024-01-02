/* SharpArgParse
 * Copyright (c) 2023 mokabe-yn <okabe_m@hmi.aitech.ac.jp>
 * 
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to
 * permit persons to whom the Software is furnished to do so, subject to
 * the following conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
 * LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
 * OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
 * WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. * 
 * 
 * */

// Note: use only C#7.3 for .NET Framework 4.7.2 only environs.

//#define ARGPARCE_EXPORT
//#define ARGPARCE_BACKPORT_NET40
//#define ARGPARCE_BACKPORT_NET45
//#define ARGPARCE_BACKPORT_SHARP7_3

#pragma warning disable CA1510

using System;
using System.Collections.Generic;

namespace SharpArgParse
{
    internal static class ArgParse<TOptions> where TOptions : new()
    {
        public static System.Reflection.PropertyInfo[] GetPropertiesInfo()
        {
            return typeof(TOptions).GetProperties(
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.SetProperty |
                System.Reflection.BindingFlags.GetProperty |
                0);
        }
        private static void Update(ref TOptions options)
        {

        }
        private static void CreateActionTable()
        {

        }
        public static ParseResult<TOptions> Parse(
            string[] args, bool allowLater = true)
        {
            var infos = GetPropertiesInfo();
            TOptions ret = new();



            throw new NotImplementedException();
        }
    }
    internal static class ArgParse
    {
        // alias ArgParse<TOptions>
        public static ParseResult<TOptions> Parse<TOptions>(
            string[] args, bool allowLater = true)
            where TOptions : new() => 
            ArgParse<TOptions>.Parse(args, allowLater);
    }
    readonly struct ParseResult<TOptions>
    {
        public TOptions Options { get; }
        public string[] RestArgs { get; }
        public ParseResult(TOptions options, string[] restArgs)
        {
            Options = options;
            RestArgs = restArgs;
        }
        public readonly void Deconstruct(out TOptions options, out string[] restArgs)
        {
            options = Options;
            restArgs = RestArgs;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    internal class AliasAttribute : Attribute
    {
        public string Alias { get; }
        public char ShortAlias { get; }
        public bool IsShortAlias { get; }

        public AliasAttribute(string alias)
        {
            if (alias is null) throw new ArgumentNullException(nameof(alias));
            Alias = alias;
            ShortAlias = '\0';
            IsShortAlias = false;
        }
        public AliasAttribute(char shortAlias)
        {
            Alias = "";
            ShortAlias = shortAlias;
            IsShortAlias = true;
        }
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    internal class ValueAliasAttribute : AliasAttribute
    {
        public object Value { get; }
        public ValueAliasAttribute(string alias, object value) 
            : base(alias)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));
            Value = value;
        }
        public ValueAliasAttribute(char shortAlias, object value) 
            : base(shortAlias)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));
            Value = value;
        }
    }

    // exceptions
#if ARGPARCE_EXPORT
    public
#else
    internal
#endif
    abstract class ArgParseException : Exception
    {
        internal ArgParseException(string message) : base(message) { }
        internal ArgParseException(string message, Exception innerException)
            : base(message, innerException) { }
    }
    internal class InternalException : ArgParseException
    {
        internal InternalException(string message) : base(message) { }
        internal InternalException(string message, Exception innerException)
            : base(message, innerException) { }
    }
    internal abstract class SettingMisstakeException : ArgParseException
    {
        internal SettingMisstakeException(string message) : base(message) { }
        internal SettingMisstakeException(string message, Exception innerException)
            : base(message, innerException) { }
    }

#if ARGPARCE_EXPORT
    public
#else
    internal
#endif
    class CommandLineException : ArgParseException
    {
        internal CommandLineException(string message) : base(message) { }
        internal CommandLineException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}

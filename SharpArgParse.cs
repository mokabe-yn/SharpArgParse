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

//#define ARGPARCE_EXPORT
//#define ARGPARCE_BACKPORT_NET40
//#define ARGPARCE_BACKPORT_NET45
//#define ARGPARCE_BACKPORT_SHARP7_3

// Note: This library supports for C#7.3 and .NET Framework 4.7.2 environs.
//       This library use only C#7.3 and .NET Framework 4 features.
//       requires special attentions for cannot uses ...
//       * Span (Requires External Library at .NET Framework 4.x)
//       * unsafe (force users to allow unsafe context)
//       * Nullable class
//         This library requires legal C# 7.3 and C#8.0 syntax.
//         `string Value;` is always notnull. (by C#8.0)
//         But user inputs maybe nullable. (by C#7.3)
//         Cannot annotate nullable like `string? Value`.
//       * ValueTuple (only net47 or later)

#pragma warning disable IDE0290 // C#12: primary constructor
#pragma warning disable CA1510  // C#10: ArgumentNullException.ThrowIfNull
#pragma warning disable IDE0251 // C#8: readonly instance members
#pragma warning disable IDE0090 // C#9: target = new();
#pragma warning disable IDE0056 // C#8: Index (array[^1]);
#pragma warning disable IDE0057 // C#8: Range (array[4..8]);


using System;
using System.Collections.Generic;
using System.Reflection;

namespace SharpArgParse
{
    internal static class InternalUtility
    {
        private static bool IsLowerAlpha(char c)
            => "abcdefghijklmnopqrstuvwxyz".Contains(c);
        private static bool IsUpperAlpha(char c)
            => "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Contains(c);
        private static bool IsAlpha(char c)
            => IsUpperAlpha(c) || IsLowerAlpha(c);
        private static bool IsNumber(char c)
            => "0123456789".Contains(c);
        private static bool IsAlnum(char c)
            => IsNumber(c) || IsAlpha(c);
        private static bool IsCababElement(char c)
            => IsNumber(c) || IsLowerAlpha(c) || c == '-';

        /// <summary>ValueTuple</summary>
        internal readonly struct Value2<T1, T2>
        {
            public readonly T1 Item1 { get; }
            public readonly T2 Item2 { get; }
            public Value2(T1 t1, T2 t2)
            {
                Item1 = t1;
                Item2 = t2;
            }
            public readonly void Deconstructor(out T1 t1, out T2 t2)
            {
                t1 = Item1;
                t2 = Item2;
            }
        }

        private static IEnumerable<Value2<int, int>> CamelSplitIndices(string s)
        {
            int prev = 0;
            for(int i = 1; i < s.Length; ++i)
            {
                if (IsUpperAlpha(s[i]))
                {
                    yield return new Value2<int, int>(prev, i);
                    prev = i;
                }
            }
            yield return new Value2<int, int>(prev, s.Length);
        }

        public static bool IsPascalOrCamelCase(string s)
            => s.All(IsAlnum) && (s.Length == 0 || IsAlpha(s[0]));
        /// <summary>Is argument cabab-case?</summary>
        /// <remarks>
        /// cabab-case-is-delimited-string-by-hyphen.
        /// </remarks>
        public static bool IsCebabCase(string s)
        {
            if (s.Length == 0) return true;
            if (s[0] == '-') return false;
            if (s[s.Length - 1] == '-') return false;
            if (s.Contains("--")) return false;
            return s.All(IsCababElement);
        }
        /// <summary>
        /// convert from PascalCase or camelCase to kebab-case.
        /// </summary>
        public static string ToKebabCase(string s)
        {
            if (IsCebabCase(s)) return s;
            if (!IsPascalOrCamelCase(s))
            {
                throw new ArgumentException(
                    "string is not PascalCase or camel Case", nameof(s));
            }
            var low = s.ToLower(System.Globalization.CultureInfo.InvariantCulture);
            return string.Join("-",
                CamelSplitIndices(s)
                .Select(be => low.Substring(be.Item1, be.Item2 - be.Item1))
                );
        }
    }
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

        private static IEnumerable<Trigger> ToTrigger(PropertyInfo prop) {
            yield return new Trigger(prop);
            foreach (var attr in prop.GetCustomAttributes<AliasAttribute>())
            {
                yield return new Trigger(prop, attr);
            }
        }
        private class Trigger
        {
            public static Trigger Empty = new Trigger(
                typeof(Trigger).GetProperty(nameof(TriggerName))
                ?? throw new InternalException(""));
            public static bool IsEmpty(Trigger t) => ReferenceEquals(t, Empty);

            public PropertyInfo TargetProperty { get; }
            public bool RecieveArgument { get; }
            public string TriggerName { get; }
            public char ShortTrigger { get; }
            public bool IsShortTrigger => ShortTrigger != '\0';
            private static readonly object UnsetValue = new object();
            private readonly object _backingTargetValue;
            public object TargetValue
            {
                get
                {
                    if (ReferenceEquals(UnsetValue, _backingTargetValue))
                    {
                        throw new InternalException(
                            $"{TargetProperty.Name}({TriggerName}):" +
                            "TargetValue is not available."
                            );
                    }
                    return _backingTargetValue;
                }
            }
            public Trigger(PropertyInfo targetProperty)
            {
                _backingTargetValue = UnsetValue;
                RecieveArgument = targetProperty.PropertyType != typeof(bool);
                TargetProperty = targetProperty;

                ShortTrigger = '\0';
                TriggerName = "--" + InternalUtility.ToKebabCase(targetProperty.Name);
            }
            public Trigger(PropertyInfo targetProperty, AliasAttribute alias)
                : this(targetProperty)
            {
                if (alias.IsShortAlias)
                {
                    ShortTrigger = alias.ShortAlias;
                }
                else
                {
                    TriggerName = "--" + InternalUtility.ToKebabCase(alias.Alias);
                }

                if (alias is ValueAliasAttribute valias)
                {
                    RecieveArgument = false;
                    _backingTargetValue = valias.Value;
                }
            }
            public void Apply(object options)
            {
                // TODO: 複数受け付けるオプションへの対応
                TargetProperty.SetValue(options, _backingTargetValue);
            }
            public void Apply(object options, string stringValue)
            {
                var value = ConvertTo(TargetProperty, stringValue);
                TargetProperty.SetValue(options, value);
            }
            private static object ConvertTo(PropertyInfo pinfo, string s)
            {
                Type t = pinfo.PropertyType;
                if (t == typeof(sbyte)) return Convert.ToSByte(s);
                if (t == typeof(short)) return Convert.ToInt16(s);
                if (t == typeof(int)) return Convert.ToInt32(s);
                if (t == typeof(long)) return Convert.ToInt64(s);
                if (t == typeof(byte)) return Convert.ToByte(s);
                if (t == typeof(ushort)) return Convert.ToUInt16(s);
                if (t == typeof(uint)) return Convert.ToUInt32(s);
                if (t == typeof(ulong)) return Convert.ToUInt64(s);
                if (t == typeof(string)) return s;
                throw new SettingMisstakeException(
                    $"{pinfo.Name}: " +
                    $"Unsupported type: " +
                    $"{pinfo.PropertyType.FullName}");
            }
        }
        public static IEnumerable<T> Chain<T>(IEnumerable<IEnumerable<T>> source)
        {
            foreach (var x in source)
            {
                foreach (var y in x)
                {
                    yield return y;
                }
            }
        }
        private static Trigger UniqueLongTrigger(string arg, Trigger[] longs)
        {
            if (arg.Contains('='))
            {
                arg = arg.Split('=', 2)[0];
            }
            var ret = longs.Where(t => t.TriggerName.StartsWith(arg)).ToArray();
            if (ret.Length == 0)
            {
                throw new CommandLineException(
                    $"unknown option: {arg.Substring(2)}" +
                    $"");
            }
            if (ret.Length > 1)
            {
                var candidate = ret.Select(t => t.TriggerName);
                throw new CommandLineException(
                    $"ambiguous option: {arg.Substring(2)}" +
                    $" (could be {string.Join(" or ", candidate)})" +
                    $"");
            }
            return ret[0];
        }
        private static Trigger UniqueShortTrigger(char arg, Trigger[] shorts)
        {
            var ret = shorts.Where(t => t.ShortTrigger == arg).ToArray();
            if (ret.Length == 0)
            {
                throw new CommandLineException(
                    $"unknown option: {arg}" +
                    $"");
            }
            if (ret.Length > 1)
            {
                var candidate = ret.Select(t => t.TriggerName);
                throw new CommandLineException(
                    $"ambiguous option: {arg}" +
                    $"");
            }
            return ret[0];
        }
        private static ParseResult<TOptions> Core(
            string[] args, bool allowLater, Trigger[] shorts, Trigger[] longs)
        {
            // TODO: refactor to class-Core state machine.
            bool restonly = false;
            Trigger target = Trigger.Empty;
            object boxed_options = new TOptions();
            var rest = new List<string>();

            foreach (string a in args)
            {
                if (restonly)
                {
                    rest.Add(a);
                    continue;
                }
                if (!Trigger.IsEmpty(target))
                {
                    target.Apply(boxed_options, a);
                    target = Trigger.Empty;
                    continue;
                }
                if (a == "--")
                {
                    restonly = true;
                    continue;
                }
                if (a.StartsWith("--"))
                {
                    Trigger trigger = UniqueLongTrigger(a, longs);
                    if (!trigger.RecieveArgument)
                    {
                        trigger.Apply(boxed_options);
                        continue;
                    }
                    string[] argelement = a.Split('=', 2);
                    if (argelement.Length == 2)
                    {
                        trigger.Apply(boxed_options, argelement[1]);
                        continue;
                    }
                    else
                    {
                        // need recieve next argument
                        target = trigger;
                        continue;
                    }
                }
                if (a.StartsWith('-') && a.Length != 1)
                {
                    for (string s = a.Substring(1); s.Length != 0; s = s.Substring(1))
                    {
                        Trigger t = UniqueShortTrigger(s[0], shorts);
                        if (!t.RecieveArgument)
                        {
                            t.Apply(boxed_options);
                            continue;
                        }
                        else
                        {
                            if (s.Length != 1)
                            {
                                t.Apply(boxed_options, s.Substring(1));
                            }
                            else
                            {
                                target = t;
                            }
                            break;
                        }
                    }
                    continue;
                }
                rest.Add(a);
                restonly = !allowLater;
            }
            return new ParseResult<TOptions>(
                (TOptions)boxed_options, rest.ToArray());
        }

        public static ParseResult<TOptions> Parse(
            string[] args, bool allowLater = true)
        {
            var infos = GetPropertiesInfo();
            Trigger[] triggers = Chain(infos.Select(ToTrigger)).ToArray();

            // TODO: validate no dup.
            Trigger[] shorts = triggers.Where(t => t.IsShortTrigger).ToArray();
            Trigger[] longs = triggers.Where(t => !t.IsShortTrigger).ToArray();
            return Core(args, allowLater, shorts, longs);
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
    internal class SettingMisstakeException : ArgParseException
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

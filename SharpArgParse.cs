/* SharpArgParse v0.9.1-dev
 * Copyright (c) 2024 mokabe-yn <okabe_m@hmi.aitech.ac.jp>
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

// TODO: auto generate help.
// TODO: call help-mode: prog.exe --help

#pragma warning disable IDE0290 // C#12: primary constructor
#pragma warning disable CA1510  // C#10: ArgumentNullException.ThrowIfNull
#pragma warning disable IDE0251 // C#8: readonly instance members
#pragma warning disable IDE0090 // C#9: target = new();
#pragma warning disable IDE0056 // C#8: Index (array[^1]);
#pragma warning disable IDE0057 // C#8: Range (array[4..8]);


using System;
using System.Collections.Generic;
using System.Reflection;
using SharpArgParse.Internals;

namespace SharpArgParse.Internals
{
    internal static class StringConvert
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

        private static IEnumerable<InternalTuple<int, int>> CamelSplitIndices(string s)
        {
            int prev = 0;
            for (int i = 1; i < s.Length; ++i)
            {
                if (IsUpperAlpha(s[i]))
                {
                    yield return new InternalTuple<int, int>(prev, i);
                    prev = i;
                }
            }
            yield return new InternalTuple<int, int>(prev, s.Length);
        }
        private static string ToUpperHeadOnly(string s)
        {
            if (s.Length == 0) return s;
            return char.ToUpper(
                s[0], System.Globalization.CultureInfo.InvariantCulture) +
                s.Substring(1);
        }

        /// <summary>Is PascalCase or camelCase?</summary>
        public static bool IsPascalOrCamelCase(string s)
            => s.All(IsAlnum) && (s.Length == 0 || IsAlpha(s[0]));
        /// <summary>Is argument cabab-case?</summary>
        /// <remarks>
        /// cabab-case-is-delimited-string-by-hyphen.
        /// </remarks>
        public static bool IsKebabCase(string s)
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
            if (IsKebabCase(s)) return s;
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
        /// <summary>
        /// convert from kebab-case to PascalCase.
        /// </summary>
        public static string KebabCaseToPascalCase(string s)
        {
            if (!IsKebabCase(s))
            {
                throw new ArgumentException(
                    "string is not kebab-case", nameof(s));
            }
            var ret = s.Split('-')
                .Select(ToUpperHeadOnly);
            return string.Join("", ret);
        }
    }

    /// <summary>ValueTuple</summary>
    internal readonly struct InternalTuple<T1, T2>
    {
        public readonly T1 Item1 { get; }
        public readonly T2 Item2 { get; }
        public InternalTuple(T1 t1, T2 t2)
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
    internal static class Utility
    {
        /// <summary>
        /// convert to <paramref name="t"/> type.
        /// </summary>
        /// <exception cref="SettingMisstakeException"></exception>
        public static object ConvertTo(Type t, string s)
        {
            if (t == typeof(sbyte)) return Convert.ToSByte(s);
            if (t == typeof(short)) return Convert.ToInt16(s);
            if (t == typeof(int)) return Convert.ToInt32(s);
            if (t == typeof(long)) return Convert.ToInt64(s);
            if (t == typeof(byte)) return Convert.ToByte(s);
            if (t == typeof(ushort)) return Convert.ToUInt16(s);
            if (t == typeof(uint)) return Convert.ToUInt32(s);
            if (t == typeof(ulong)) return Convert.ToUInt64(s);
            if (t == typeof(string)) return s;
            if (t.IsEnum)
            {
                string k = StringConvert.KebabCaseToPascalCase(s);
                return Enum.Parse(t, k);
            }
            throw new ArgumentException($"Unsupported type: {t.FullName}");
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
    }
    internal class Trigger
    {
        public static Trigger Empty = new Trigger(
            typeof(Trigger).GetProperty(nameof(TriggerName))
            ?? throw new InternalException(""));
        public static bool IsEmpty(Trigger t) => ReferenceEquals(t, Empty);

        public PropertyInfo TargetProperty { get; }
        public Type ElementType { get; }
        public bool IsMultiArgument { get; }
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
            bool isbool = targetProperty.PropertyType == typeof(bool);
            _backingTargetValue = isbool ? true : UnsetValue;
            RecieveArgument = !isbool;
            TargetProperty = targetProperty;

            // T[] => T, T => T
            Type type = TargetProperty.PropertyType;
            IsMultiArgument = type.IsArray;
            ElementType = IsMultiArgument ?
                type.GetElementType() ?? throw new InternalException("") :
                type;

            ShortTrigger = '\0';
            TriggerName = "--" + StringConvert.ToKebabCase(targetProperty.Name);
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
                TriggerName = "--" + StringConvert.ToKebabCase(alias.Alias);
            }

            if (alias is ValueAliasAttribute valias)
            {
                RecieveArgument = false;
                _backingTargetValue = valias.Value;
            }
        }
        private void ApplyValue(object options, object value)
        {
            if (IsMultiArgument)
            {
                // TODO: make T[] every time: O(N^2) => use List<T>: O(N)
                // [1,2,3] => [1,2,3,value]
                Array old = (Array)(TargetProperty.GetValue(options)
                    ?? Array.CreateInstance(ElementType, 0));
                Array dst = Array.CreateInstance(
                    ElementType, old.Length + 1);
                old.CopyTo(dst, 0);
                dst.SetValue(value, old.Length);
                TargetProperty.SetValue(options, dst);
            }
            else
            {
                TargetProperty.SetValue(options, value);
            }
        }
        public void Apply(object options)
            => ApplyValue(options, _backingTargetValue);
        public void Apply(object options, string stringValue)
        {
            object value;
            try
            {
                value = Utility.ConvertTo(ElementType, stringValue);
            }
            catch (Exception ex)
            {
                throw new CommandLineException(
                    $"{TriggerName}: {ex.Message}", ex);
            }
            ApplyValue(options, value);
        }
    }
    internal class ArgumentReciever<TOptions>
    {
        private Trigger[] Shorts { get; }
        private Trigger[] Longs { get; }
        private bool AllowLater { get; }

        private bool RestOnlyMode { get; set; } = false;
        private Trigger CurrentTarget { get; set; } = Trigger.Empty;
        private List<string> RestArgument { get; } = new List<string>();
        // (boxed if struct) options instance
        private object Options { get; }
        public TOptions GetOptions() => (TOptions)Options;
        public string[] GetRest() => RestArgument.ToArray();
        public void Validate()
        {
            if (!Trigger.IsEmpty(CurrentTarget))
            {
                throw new CommandLineException(
                    $"option {CurrentTarget.TriggerName} requires a value.");
            }
        }

        private Trigger UniqueLongTrigger(string arg)
        {
            if (arg.Contains('='))
            {
                arg = arg.Split('=', 2)[0];
            }
            var ret = Longs.Where(t => t.TriggerName.StartsWith(arg)).ToArray();
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
        private Trigger UniqueShortTrigger(char arg)
        {
            var ret = Shorts.Where(t => t.ShortTrigger == arg).ToArray();
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

        public ArgumentReciever(
            object options, bool allowLater, Trigger[] shorts, Trigger[] longs)
        {
            Options = options;
            AllowLater = allowLater;
            Shorts = shorts;
            Longs = longs;
        }
        public void Next(string arg)
        {
            Action<string> action =
                RestOnlyMode ? RestAdd :
                !Trigger.IsEmpty(CurrentTarget) ? RecieveRemainedArgument :
                arg == "--" ? TerminateOption :
                arg.StartsWith("--") ? ApplyLongOption :
                arg.StartsWith('-') ? ApplyShortOption :
                ApplyRestArgument;
            action(arg);
        }

        private void RestAdd(string arg)
            => RestArgument.Add(arg);
        private void RecieveRemainedArgument(string arg)
        {
            CurrentTarget.Apply(Options, arg);
            CurrentTarget = Trigger.Empty;
        }
        private void TerminateOption(string _)
            => RestOnlyMode = true;
        private void ApplyLongOption(string arg)
        {
            Trigger trigger = UniqueLongTrigger(arg);
            if (!trigger.RecieveArgument)
            {
                trigger.Apply(Options);
                return;
            }
            string[] a01 = arg.Split('=', 2);
            if (a01.Length == 2)
            {
                trigger.Apply(Options, a01[1]);
            }
            else
            {
                // need recieve next argument
                CurrentTarget = trigger;
            }
        }
        private void ApplyShortOption(string arg)
        {
            for (string s = arg.Substring(1); s.Length != 0; s = s.Substring(1))
            {
                Trigger t = UniqueShortTrigger(s[0]);
                if (!t.RecieveArgument)
                {
                    // no argument option
                    // grep -niE
                    t.Apply(Options);
                }
                else
                {
                    // recieve argument option
                    if (s.Length != 1)
                    {
                        // not splited : grep -ePATTERN
                        t.Apply(Options, s.Substring(1));
                    }
                    else
                    {
                        // splited : grep -e PATTERN
                        CurrentTarget = t;
                    }
                    return;
                }
            }
        }
        private void ApplyRestArgument(string arg)
        {
            RestArgument.Add(arg);
            RestOnlyMode = !AllowLater;
        }
    }
}

namespace SharpArgParse
{
    internal static class ArgParse<TOptions> where TOptions : new()
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

        private static IEnumerable<Trigger> ToTrigger(PropertyInfo prop) {
            yield return new Trigger(prop);
            foreach (var attr in prop.GetCustomAttributes<AliasAttribute>())
            {
                yield return new Trigger(prop, attr);
            }
        }

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

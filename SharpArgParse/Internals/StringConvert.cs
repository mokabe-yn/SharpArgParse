using System;
using System.Collections.Generic;
using System.Linq;

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
}
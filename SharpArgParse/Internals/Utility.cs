using System;
using System.Collections.Generic;

namespace SharpArgParse.Internals
{
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
}
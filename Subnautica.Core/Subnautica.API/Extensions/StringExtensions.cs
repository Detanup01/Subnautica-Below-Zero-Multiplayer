﻿namespace Subnautica.API.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    public static class StringExtensions
    {
        public static T ToEnum<T>(this string value)
        {
            try
            {
                return (T) Enum.Parse(typeof(T), value, true);
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public static string ToInvariantCultureString(this double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public static string ToInvariantCultureString(this float value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public static bool IsNotNull(this string value)
        {
            return !value.IsNull();
        }

        public static bool IsNull(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(this T[] array, int size)
        {
            for (var i = 0; i < (float) array.Length / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(this List<T> list, int size)
        {
            for (var i = 0; i < (float)list.Count / size; i++)
            {
                yield return list.Skip(i * size).Take(size);
            }
        }

        public static IEnumerable<Dictionary<int, long>> Split(this Dictionary<int, long> list, int size)
        {
            for (var i = 0; i < (float)list.Count / size; i++)
            {
                yield return list.Skip(i * size).Take(size).ToDictionary(k => k.Key, v => v.Value);
            }
        }

        public static IEnumerable<Dictionary<ushort, byte>> Split(this Dictionary<ushort, byte> list, int size)
        {
            for (var i = 0; i < (float)list.Count / size; i++)
            {
                yield return list.Skip(i * size).Take(size).ToDictionary(k => k.Key, v => v.Value);
            }
        }

        public static bool IsOverride(this MethodInfo methodInfo)
        {
            return (methodInfo.GetBaseDefinition().DeclaringType != methodInfo.DeclaringType);
        }

        public static bool IsWorldStreamer(this string uniqueId)
        {
            return uniqueId.IsNotNull() && uniqueId.Contains("WorldStreamer_");
        }









        public static int WorldStreamerToSlotId(this string uniqueId)
        {
            return Convert.ToInt32(uniqueId.Replace("WorldStreamer_", ""));
        }

        public static string ToWorldStreamerId(this int slotId)
        {
            return string.Format("WorldStreamer_" + slotId);
        }
    }
}

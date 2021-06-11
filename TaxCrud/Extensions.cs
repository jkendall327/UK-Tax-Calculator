using System;
using System.Collections.Generic;
using System.Linq;

namespace TaxCrud
{
    internal static class Extensions
    {
        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int n)
            => source.Skip(Math.Max(0, source.Count() - n));

        public static bool IsEmpty<T>(this IEnumerable<T> source) => !source.Any();

        public static string Truncate(this string value, int maxLength)
        {
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    public static class Extensions
    {
        public static T GetItemWithMin<T>(this IEnumerable<T> collection, Func<T, int> valueSelector, T defaultValue = default(T))
        {
            Dictionary<T, int> itemValues = new Dictionary<T, int>();
            foreach (var x in collection)
            {
                itemValues[x] = valueSelector(x);
            }

            var ordered = itemValues.OrderBy(x => x.Value);
            var min = ordered.First().Value;

            if (ordered.Count(x => x.Value == min) > 1)
                return defaultValue;

            return ordered.First().Key;
        }

        public static void AddUnique<T>(this ICollection<T> col, T value)
        {
            if (!col.Contains(value))
                col.Add(value);
        }

        public static int MaxOrDefault<T>(this IEnumerable<T> col, Func<T, int> valueSelector, int defaultValue = 0)
        {
            var list = col.ToList();
            if (list == null || list.Count() == 0)
            {
                return defaultValue;
            }

            return list.Max(valueSelector);
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key)
        {
            return GetValueOrDefault(dictionary, key, default(TValue));
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            var outputValue = defaultValue;
            if (dictionary.ContainsKey(key))
            {
                var foundValue = dictionary[key];
                outputValue = foundValue;
            }
            return outputValue;
        }

        public static IEnumerable<T> WithoutNull<T>(this IEnumerable<T> input)
        {
            return input.Where(x => x != null);
        }
    }

}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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

        static readonly Random random = new Random();
        public static T GetRandom<T>(this List<T> collection)
        {
           return collection[random.Next(collection.Count)];
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

        public static void Draw(this Dictionary<Point, string> dictionaryToDraw, TextWriter writer, string missingCharacter = "", int borderThickness = 0)
        {
            int offsetX = -dictionaryToDraw.Min(x => x.Key.X) + borderThickness;
            int offsetY = -dictionaryToDraw.Min(x => x.Key.Y) + borderThickness;
            int width = dictionaryToDraw.Max(x => x.Key.X) + offsetX + 1 + borderThickness;
            int height = dictionaryToDraw.Max(x => x.Key.Y) + offsetY + 1 + borderThickness;

            string[,] toDraw = new string[width, height];

            foreach (var item in dictionaryToDraw)
                toDraw[item.Key.X + offsetX, item.Key.Y + offsetY] = item.Value;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                    writer.Write(toDraw[x, y] ?? missingCharacter);
                writer.WriteLine();
            }
        }

        public static void Draw(this Dictionary<Point, char> dictionaryToDraw, TextWriter writer, char missingCharacter = default(char), int borderThickness = 0)
        {
            dictionaryToDraw.ToDictionary(x => x.Key, x => x.Value.ToString()).Draw(writer, missingCharacter == default(char) ? string.Empty : missingCharacter.ToString(), borderThickness);
        }

        public static Point Move(this Point point, int x, int y)
        {
            return new Point(point.X + x, point.Y + y);
        }
    }

}
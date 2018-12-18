using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode
{
    public class Day5
    {
        public static void Run()
        {
            var polymer = File.ReadAllText(@"Day5\Input.txt").Trim();
            var allTypes = polymer.ToLower().Distinct();
            Dictionary<char, int> typeRetractions = new Dictionary<char, int>();
            foreach (var type in allTypes)
            {
                Stack<char> stack = new Stack<char>();
                var currentPolymer = polymer.Replace(type.ToString(), "").Replace(char.ToUpper(type).ToString(), "");
                foreach (var current in currentPolymer)
                {
                    if (stack.Count == 0)
                    {
                        stack.Push(current);
                        continue;
                    }

                    var last = stack.Peek();
                    if (char.ToUpper(current) == char.ToUpper(last) && current != last)
                    {
                        stack.Pop();
                        continue;
                    }


                    stack.Push(current);

                }
                typeRetractions[type] = stack.Count;
            }


            foreach (var item in typeRetractions.OrderBy(x => x.Value))
            {
                Console.WriteLine($"{item.Key}: {item.Value}");
            }
        }
    }
}

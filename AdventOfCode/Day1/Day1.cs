using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode
{
    public class Day1
    {
        public static void Run()
        {
            var numbers = File.ReadAllLines(@"Day1\Input.txt").Select(x => int.Parse(x)).ToList();
            Console.WriteLine($"Part 1: {numbers.Sum()}");

            HashSet<int> knownFrequencies = new HashSet<int>();
            int currentFrequency = 0;
            while (true)
            {
                currentFrequency += numbers[knownFrequencies.Count % numbers.Count];
                if (knownFrequencies.Contains(currentFrequency))
                    break;
                knownFrequencies.Add(currentFrequency);
            }

            Console.WriteLine($"Part 2: {currentFrequency}");
        }
    }
}

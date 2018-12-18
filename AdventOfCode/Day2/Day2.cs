using System;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode
{
    public class Day2
    {
        public static void Run()
        {
            var boxIds = File.ReadAllLines(@"Day2\Input.txt");
            const string ALPHABET = "abcdefghijklmnopqrstuvwxyz";
            int boxesWithLetterTwice = 0;
            int boxesWithLetterThrice = 0;
            foreach (var boxId in boxIds)
            {
                if (ALPHABET.Any(x => boxId.Count(c => c == x) == 2))
                    boxesWithLetterTwice += 1;

                if (ALPHABET.Any(x => boxId.Count(c => c == x) == 3))
                    boxesWithLetterThrice += 1;
            }

            Console.WriteLine($"Part 1: {boxesWithLetterTwice * boxesWithLetterThrice}");

            foreach (var box1 in boxIds)
            {
                foreach (var box2 in boxIds)
                {
                    if (box1 == box2)
                        continue;

                    var resultBuilder = new StringBuilder();
                    for (int i = 0; i < box1.Length; i++)
                    {
                        if (box1[i] == box2[i])
                            resultBuilder.Append(box1[i]);

                    }

                    var result = resultBuilder.ToString();
                    if (result.Length + 1 == box1.Length)
                    {
                        Console.WriteLine($"Part 2: {result}");

                        return;
                    }
                }
            }
        }
    }
}

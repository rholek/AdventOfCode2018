using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode
{
    public class Day3
    {
        public static void Run()
        {
            List<int>[,] sheet = new List<int>[1000, 1000];

            foreach (var claim in File.ReadAllLines(@"Day3\Input.txt"))
            {
                var split = claim.Split(' ');
                int id = int.Parse(split[0].Substring(1));
                int xStart = int.Parse(split[2].Split(',')[0]);
                int yStart = int.Parse(split[2].Split(',')[1].Trim(':'));
                int width = int.Parse(split[3].Split('x')[0]);
                int height = int.Parse(split[3].Split('x')[1]);

                for (int x = xStart; x < xStart + width; x++)
                {
                    for (int y = yStart; y < yStart + height; y++)
                    {
                        if (sheet[x, y] == null)
                            sheet[x, y] = new List<int>();
                        sheet[x, y].Add(id);
                    }
                }
            }

            Console.WriteLine($"Part 1: {sheet.Cast<List<int>>().WithoutNull().Count(x => x.Count > 1)}");
            Console.WriteLine($"Part 2: {sheet.Cast<List<int>>().WithoutNull().SelectMany(x => x).Except(sheet.Cast<List<int>>().WithoutNull().Where(x => x.Count > 1).SelectMany(x => x)).Single()}");
        }
    }
}

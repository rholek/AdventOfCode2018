using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode
{
    public class Day6
    {
        public static void Run()
        {
            List<Day6Point> points = new List<Day6Point>();
            int curId = 0;
            foreach (var item in File.ReadAllLines(@"Day6\Input.txt"))
            {
                var coordinates = item.Split(',');
                var x = int.Parse(coordinates[0].Trim());
                var y = int.Parse(coordinates[1].Trim());
                curId++;
                points.Add(new Day6Point(x, y, curId.ToString()));
            }

            var minx = points.Min(x => x.X);
            var miny = points.Min(x => x.Y);

            var clone = points.ToList();
            points.Clear();
            foreach (var item in clone)
            {
                points.Add(new Day6Point(item.X - minx, item.Y - miny, item.Id));
            }

            var maxx = points.Max(x => x.X) + 1;
            var maxy = points.Max(x => x.Y) + 1;


            //part 1
            string[,] data = new string[maxx, maxy];
            for (int x = 0; x < maxx; x++)
            {
                for (int y = 0; y < maxy; y++)
                {
                    data[x, y] = points.GetItemWithMin(z => z.GetDistance(x, y))?.Id ?? "-1";
                }
            }

            List<string> ignored = new List<string>();

            for (int x = 0; x < maxx; x++)
            {
                ignored.AddUnique(data[x, 0]);
                ignored.AddUnique(data[x, maxy - 1]);
            }

            for (int y = 0; y < maxy; y++)
            {
                ignored.AddUnique(data[0, y]);
                ignored.AddUnique(data[maxx - 1, y]);
            }

            Dictionary<string, int> count = new Dictionary<string, int>();
            for (int x = 0; x < maxx; x++)
            {
                for (int y = 0; y < maxy; y++)
                {
                    if (!count.ContainsKey(data[x, y]))
                        count[data[x, y]] = 0;

                    count[data[x, y]] += 1;
                }
            }

            var maxArea = count.OrderByDescending(x => x.Value).Where(x => !ignored.Contains(x.Key)).First();
            Console.WriteLine(maxArea.Value);

            //part 2
            int[,] data2 = new int[maxx, maxy];
            for (int x = 0; x < maxx; x++)
            {
                for (int y = 0; y < maxy; y++)
                {
                    data2[x, y] = points.Sum(z => z.GetDistance(x, y));
                }
            }

            const int X = 10000;
            int countOfPointWithLessThanX = 0;

            for (int x = 0; x < maxx; x++)
            {
                for (int y = 0; y < maxy; y++)
                {
                    if (data2[x, y] < X)
                        countOfPointWithLessThanX++;
                }
            }

            Console.WriteLine(countOfPointWithLessThanX);

        }

        class Day6Point
        {
            public int X { get; }
            public int Y { get; }

            public string Id { get; }

            public Day6Point(int x, int y, string id)
            {
                X = x;
                Y = y;
                Id = id;
            }

            public int GetDistance(int x, int y)
            {
                return Math.Abs(X - x) + Math.Abs(Y - y);
            }
        }
    }
}

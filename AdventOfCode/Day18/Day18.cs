using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace AdventOfCode
{
    public class Day18
    {
        public static void Run()
        {
            var lines = File.ReadAllLines(@"Day18\Input.txt");
            Dictionary<Point, AcreeType> map = new Dictionary<Point, AcreeType>();
            for (var y = 0; y < lines.Length; y++)
            {
                var line = lines[y];
                for (var x = 0; x < line.Length; x++)
                {
                    map[new Point(x, y)] = Parse(line[x]);
                }

                AcreeType Parse(char acreType)
                {
                    switch (acreType)
                    {
                        case '.':
                            return AcreeType.OpenGround;
                        case '|':
                            return AcreeType.Wooded;
                        case '#':
                            return AcreeType.LumberYard;
                        default:
                            throw new Exception("I don't know what this symbol means. Maybe ice, since we are at the north pole?");
                    }
                }
            }

            int yMax = map.Max(x => x.Key.Y);
            int xMax = map.Max(x => x.Key.X);
            const int MINUTES = 1000000000;
            for (int minute = 1; minute <= MINUTES; minute++)
            {
                var mapClone = map.ToDictionary(x => x.Key, x => x.Value);
                foreach (var item in map)
                {
                    switch (item.Value)
                    {
                        case AcreeType.OpenGround:
                            mapClone[item.Key] = GetAdjectedAcreTypes(item.Key).Count(x => x == AcreeType.Wooded) >= 3
                                ? AcreeType.Wooded
                                : AcreeType.OpenGround;
                            break;
                        case AcreeType.Wooded:
                            mapClone[item.Key] =
                                GetAdjectedAcreTypes(item.Key).Count(x => x == AcreeType.LumberYard) >= 3
                                    ? AcreeType.LumberYard
                                    : AcreeType.Wooded;
                            break;
                        case AcreeType.LumberYard:
                            mapClone[item.Key] =
                                GetAdjectedAcreTypes(item.Key).Count(x => x == AcreeType.Wooded) >= 1 &&
                                GetAdjectedAcreTypes(item.Key).Count(x => x == AcreeType.LumberYard) >= 1
                                    ? AcreeType.LumberYard
                                    : AcreeType.OpenGround;
                            break;
                    }
                }

                map = mapClone;

                if (minute % 100 == 0)
                {
                    int woodedAcres = map.Count(x => x.Value == AcreeType.Wooded);
                    int lumberYardsAcres = map.Count(x => x.Value == AcreeType.LumberYard);
                    int total = woodedAcres * lumberYardsAcres;
                    Console.WriteLine($"{minute}:{total}");

                    //repeat period of 700
                    //item 1000 == item 1000000000
                }
            }
            

            AcreeType GetValue(int x, int y)
            {
                return map.GetValueOrDefault(new Point(x, y), AcreeType.Nothing);
            }

            List<AcreeType> GetAdjectedAcreTypes(Point point)
            {
                return GetAdjectedAcres(point).Select(a => GetValue(a.X, a.Y))
                    .Where(a => a != AcreeType.Nothing).ToList();
            }

            IEnumerable<Point> GetAdjectedAcres(Point affectedPoint)
            {
                yield return new Point(affectedPoint.X - 1, affectedPoint.Y);
                yield return new Point(affectedPoint.X + 1, affectedPoint.Y);
                yield return new Point(affectedPoint.X, affectedPoint.Y - 1);
                yield return new Point(affectedPoint.X, affectedPoint.Y + 1);
                yield return new Point(affectedPoint.X - 1, affectedPoint.Y - 1);
                yield return new Point(affectedPoint.X - 1, affectedPoint.Y + 1);
                yield return new Point(affectedPoint.X + 1, affectedPoint.Y - 1);
                yield return new Point(affectedPoint.X + 1, affectedPoint.Y + 1);
            }

            Draw();

            void Draw()
            {

                for (int y = 0; y <= yMax; y++)
                {
                    for (int x = 0; x <= xMax; x++)
                        Console.Write(GetValue(x, y).Draw());
                    Console.WriteLine();
                }
            }
        }
    }

    enum AcreeType
    {
        Nothing,
        OpenGround,
        Wooded,
        LumberYard
    }

    static class AcreeTypeExtensions
    {
        public static string Draw(this AcreeType ss)
        {
            switch (ss)
            {
                case AcreeType.OpenGround:
                    return ".";
                case AcreeType.Wooded:
                    return "|";
                case AcreeType.LumberYard:
                    return "#";
                default:
                    return null;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace AdventOfCode
{
    public class Day17
    {
        public static void Run()
        {
            var lines = File.ReadAllLines(@"Day17\Input.txt");
            Dictionary<Point, SquareType> map = new Dictionary<Point, SquareType>();
            foreach (var line in lines)
            {
                var lineResult = ParseLine(line);

                for (int x = lineResult.xFrom; x <= lineResult.xTo; x++)
                {
                    for (int y = lineResult.yFrom; y <= lineResult.yTo; y++)
                    {
                        map[new Point(x, y)] = SquareType.Clay;
                    }
                }

                (int xFrom, int xTo, int yFrom, int yTo) ParseLine(string lineToParse)
                {
                    var split = lineToParse.Split(',');
                    split = split.Select(xx => xx.Trim()).OrderBy(xx => xx[0]).ToArray();
                    var x = split[0].Trim().Split('=')[1].Trim();
                    var xFrom = int.Parse(x.Split('.').First());
                    var xTo = int.Parse(x.Split('.').Last());
                    var y = split[1].Trim().Split('=')[1].Trim();
                    var yFrom = int.Parse(y.Split('.').First());
                    var yTo = int.Parse(y.Split('.').Last());
                    return (xFrom, xTo, yFrom, yTo);
                }
            }

            int yMax = map.Max(x => x.Key.Y);
            int yMin = map.Min(x => x.Key.Y);
            int xMax = map.Max(x => x.Key.X) + 1;
            int xMin = map.Min(x => x.Key.X) - 1;

            map[new Point(500, 1)] = SquareType.WaterFlowing;

            var pointsToProcess = GetPossiblyAffectedCells(new Point(500, 1)).ToList();

            while (true)
            {
                var newnewFlowingPoints = new List<Point>();
                foreach (var newPOint in pointsToProcess)
                {
                    var squareMeter = GetValue(newPOint.X, newPOint.Y);
                    if (squareMeter != SquareType.WaterFlowing)
                        continue;

                    var keyX = newPOint.X;
                    var keyY = newPOint.Y;
                    if (keyY == yMax)
                        continue;

                    var under = GetValue(keyX, keyY + 1);
                    var left = keyX == xMin ? SquareType.Border : GetValue(keyX - 1, keyY);
                    var right = keyX == xMax ? SquareType.Border : GetValue(keyX + 1, keyY);

                    if (under == SquareType.WaterFlowing)
                        continue;

                    if (under == SquareType.Sand)
                    {
                        var point = new Point(keyX, keyY + 1);
                        map[point] = SquareType.WaterFlowing;
                        newnewFlowingPoints.AddRange(GetPossiblyAffectedCells(point));
                    }
                    else
                    {
                        if (left == SquareType.Sand)
                        {
                            var point = new Point(keyX - 1, keyY);
                            map[point] = SquareType.WaterFlowing;
                            newnewFlowingPoints.AddRange(GetPossiblyAffectedCells(point));
                        }

                        if (right == SquareType.Sand)
                        {
                            var point = new Point(keyX + 1, keyY);
                            map[point] = SquareType.WaterFlowing;
                            newnewFlowingPoints.AddRange(GetPossiblyAffectedCells(point));
                        }
                    }

                    if (under == SquareType.Clay || under == SquareType.WaterStill)
                    {
                        int xLeftNew = keyX;
                        while (xLeftNew > xMin &&
                               GetValue(xLeftNew, keyY) == SquareType.WaterFlowing)
                            xLeftNew--;

                        int xRightNew = keyX;
                        while (xRightNew < xMax &&
                               GetValue(xRightNew, keyY) == SquareType.WaterFlowing)
                            xRightNew++;

                        var ll = GetValue(xLeftNew, keyY);
                        var rr = GetValue(xRightNew, keyY);
                        if (ll == SquareType.Clay && rr == SquareType.Clay)
                        {
                            for (int j = xLeftNew + 1; j < xRightNew; j++)
                            {
                                map[new Point(j, keyY)] = SquareType.WaterStill;
                                newnewFlowingPoints.AddRange(GetPossiblyAffectedCells(new Point(j, keyY)));
                            }
                        }

                    }
                }

                pointsToProcess = newnewFlowingPoints;
                if (pointsToProcess.Count == 0)
                    break;
            }

            SquareType GetValue(int x, int y)
            {
                return map.GetValueOrDefault(new Point(x, y), SquareType.Sand);
            }

            IEnumerable<Point> GetPossiblyAffectedCells(Point affectedPoint)
            {
                yield return new Point(affectedPoint.X - 1, affectedPoint.Y);
                yield return new Point(affectedPoint.X + 1, affectedPoint.Y);
                yield return new Point(affectedPoint.X, affectedPoint.Y - 1);
                yield return new Point(affectedPoint.X, affectedPoint.Y + 1);
                yield return affectedPoint;
            }

            // Draw();
            Console.WriteLine(map.Count(x => x.Key.Y >= yMin && (x.Value == SquareType.WaterStill)));

            void Draw()
            {
                using (StreamWriter s = new StreamWriter(new FileStream("result.txt", FileMode.OpenOrCreate)))
                {
                    for (int y = 0; y <= yMax; y++)
                    {
                        for (int x = xMin; x <= xMax; x++)
                            s.Write(map.GetValueOrDefault(new Point(x, y), SquareType.Sand).Draw());
                        s.WriteLine();
                    }
                }
            }
        }
    }

    enum SquareType
    {
        Sand,
        Clay,
        WaterFlowing,
        WaterStill,
        Border
    }

    static class SquareTypeExtensions
    {
        public static string Draw(this SquareType ss)
        {
            switch (ss)
            {
                case SquareType.Sand:
                    return ".";
                case SquareType.Clay:
                    return "#";

                case SquareType.WaterFlowing:
                    return "|";
                case SquareType.WaterStill:
                    return "~";
                default:
                    return string.Empty;
            }
        }

        public static bool IsWall(this SquareType ss)
        {
            return ss == SquareType.WaterStill || ss == SquareType.Clay;
        }
    }
}

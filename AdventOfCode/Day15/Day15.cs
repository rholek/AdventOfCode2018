using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode
{
    public class Day15
    {
        public static void Run()
        {
            int elvesAttackPower = 3;
            (int fullyFinishedRounds, int totalHitpoints, bool anyElfDied) result;
            do
            {
                elvesAttackPower++;
                result = ProcessGame("Day15/Input.txt", elvesAttackPower);
                Console.WriteLine($"Attack power:{elvesAttackPower}");
            } while (result.anyElfDied);

            Console.WriteLine($"{result.fullyFinishedRounds}*{result.totalHitpoints}={result.fullyFinishedRounds * result.totalHitpoints}");
        }

        public static (int fullyFinishedRounds, int totalHitpoints, bool anyElfDied) ProcessGame(string pathToInputFile, int elvesAttackPower = 3)
        {
            var lines = File.ReadAllLines(pathToInputFile);
            Day15Map game = new Day15Map(lines[0].Length, lines.Length);
            for (int y = 0; y < lines.Length; y++)
            {
                var line = lines[y];
                for (int x = 0; x < line.Length; x++)
                {
                    switch (line[x])
                    {
                        case '#':
                            game.Set(x, y);
                            break;
                        case 'G':
                            game.AddUnit(new Day15Unit(Day15UnitType.Goblin, new Point(x, y), 3));
                            break;
                        case 'E':
                            game.AddUnit(new Day15Unit(Day15UnitType.Elf, new Point(x, y), elvesAttackPower));
                            break;
                    }
                }
            }

            int elvesonTheBattleField = game.AllUnits.Where(x => x.Type == Day15UnitType.Elf).Count();

            int roundCounter = 0;

            PrintAndWait();
            while (!game.MakeARound())
            {
                roundCounter++;
                PrintAndWait();
            }

            PrintAndWait();

            void PrintAndWait()
            {
                //game.Print();
                //Console.WriteLine(roundCounter);
                //Console.ReadLine();
            }

            return (roundCounter, game.AllUnits.Sum(x => x.hitPoints), elvesonTheBattleField != game.AllUnits.Where(x => x.Type == Day15UnitType.Elf).Count());
        }

        #region Day 15          

        class Day15Map
        {

            List<Day15Unit> units = new List<Day15Unit>();

            bool[,] map;

            public Day15Map(int width, int height)
            {
                map = new bool[width, height];
            }

            public (Point firstStep, int cost)? GetPath(Point source, Point target)
            {
                List<Point> offsets = new List<Point>() { new Point(-1, 0), new Point(1, 0), new Point(0, 1), new Point(0, -1) };
                List<(Point firstStep, int length)?> result = new List<(Point firstStep, int length)?>();
                var occupiedPositions = new HashSet<Point>(units.Where(x => !x.IsDead).Select(x => x.Position));
                foreach (var offset in offsets)
                {
                    var copy = source;
                    copy.Offset(offset);
                    if (occupiedPositions.Contains(copy))
                        continue;
                    if (copy.Equals(target))
                        result.Add((copy, 1));
                    else
                    {
                        if (copy.X > 0 && copy.X < map.GetLength(0) && copy.Y > 0 && copy.Y < map.GetLength(1) && !map[copy.X, copy.Y])
                        {
                            var cost = GetPathLength(copy, target);
                            if (cost != null)
                                result.Add((copy, cost.Value + 1));
                        }
                    }
                }

                if (!result.Any())
                    return null;
                var min = result.Min(x => x.Value.length);
                result.RemoveAll(x => x.Value.length != min);
                return result.OrderBy(x => x.Value.firstStep.Y).ThenBy(x => x.Value.firstStep.X).FirstOrDefault();


                int? GetPathLength(Point from, Point to)
                {
                    int?[,] lenghts = new int?[map.GetLength(0), map.GetLength(1)];
                    for (int y = 0; y < map.GetLength(1); y++)
                        for (int x = 0; x < map.GetLength(0); x++)
                            lenghts[x, y] = map[x, y] ? -1 : (int?)null;

                    foreach (var unit in AllUnits)
                        lenghts[unit.Position.X, unit.Position.Y] = -1;

                    lenghts[from.X, from.Y] = 0;
                    lenghts[to.X, to.Y] = null;
                    int currentLength = 0;
                    bool anyFound = true;
                    while (anyFound)
                    {
                        anyFound = false;
                        for (int y = 0; y < map.GetLength(1); y++)
                        {
                            for (int x = 0; x < map.GetLength(0); x++)
                            {
                                if (lenghts[x, y] == currentLength)
                                {
                                    anyFound = true;
                                    if (x < map.GetLength(0) - 1 && lenghts[x + 1, y] == null)
                                        lenghts[x + 1, y] = currentLength + 1;
                                    if (x > 1 && lenghts[x - 1, y] == null)
                                        lenghts[x - 1, y] = currentLength + 1;
                                    if (y < map.GetLength(1) - 1 && lenghts[x, y + 1] == null)
                                        lenghts[x, y + 1] = currentLength + 1;
                                    if (y > 1 && lenghts[x, y - 1] == null)
                                        lenghts[x, y - 1] = currentLength + 1;
                                }
                            }
                        }

                        currentLength++;
                    }

                    return lenghts[to.X, to.Y];

                }
            }

            public void AddUnit(Day15Unit unit)
            {
                units.Add(unit);
            }

            public IEnumerable<Day15Unit> AllUnits => units.Where(x => !x.IsDead);

            public void Set(int x, int y)
            {
                map[x, y] = true;
            }

            public bool MakeARound()
            {
                var allUnits = units.OrderBy(x => x.Position.Y).ThenBy(x => x.Position.X).ToList();
                foreach (var unit in allUnits)
                {
                    if (unit.IsDead)
                        continue;

                    if (!unit.TakeATurn(this))
                        return true;
                }

                units.RemoveAll(x => x.IsDead);

                return false;
            }

            public List<(Day15Unit unit, bool canBeTargeted)> GetAllTargets()
            {
                bool[,] fullMap = new bool[map.GetLength(0), map.GetLength(1)];
                for (int y = 0; y < map.GetLength(1); y++)
                    for (int x = 0; x < map.GetLength(0); x++)
                        fullMap[x, y] = map[x, y];

                foreach (var unit in units)
                    fullMap[unit.Position.X, unit.Position.Y] = true;

                return units.Select(unit => (unit, CanTarget(unit))).OrderBy(x => x.Item1.Position.Y).ThenBy(x => x.Item1.Position.X).ToList();

                bool CanTarget(Day15Unit u)
                {
                    {
                        var x = u.Position.X;
                        var y = u.Position.Y;

                        if (x < fullMap.GetLength(0) - 1 && !fullMap[x + 1, y])
                            return true;
                        if (x > 1 && !fullMap[x - 1, y])
                            return true;
                        if (y < fullMap.GetLength(1) - 1 && !fullMap[x, y + 1])
                            return true;
                        if (y > 1 && !fullMap[x, y - 1])
                            return true;

                        return false;
                    }
                }
            }

            public Dictionary<Point, int> GetCosts(Point startPoint)
            {
                int?[,] lenghts = new int?[map.GetLength(0), map.GetLength(1)];
                for (int y = 0; y < map.GetLength(1); y++)
                    for (int x = 0; x < map.GetLength(0); x++)
                        lenghts[x, y] = map[x, y] ? -1 : (int?)null;

                foreach (var unit in units.Where(x => !x.IsDead))
                    lenghts[unit.Position.X, unit.Position.Y] = -1;

                lenghts[startPoint.X, startPoint.Y] = 0;
                int currentLength = 0;
                bool anyFound = true;
                while (anyFound)
                {
                    anyFound = false;
                    for (int y = 0; y < map.GetLength(1); y++)
                    {
                        for (int x = 0; x < map.GetLength(0); x++)
                        {
                            if (lenghts[x, y] == currentLength)
                            {
                                anyFound = true;
                                if (x < map.GetLength(0) - 1 && lenghts[x + 1, y] == null)
                                    lenghts[x + 1, y] = currentLength + 1;
                                if (x > 1 && lenghts[x - 1, y] == null)
                                    lenghts[x - 1, y] = currentLength + 1;
                                if (y < map.GetLength(1) - 1 && lenghts[x, y + 1] == null)
                                    lenghts[x, y + 1] = currentLength + 1;
                                if (y > 1 && lenghts[x, y - 1] == null)
                                    lenghts[x, y - 1] = currentLength + 1;
                            }
                        }
                    }
                    currentLength++;
                }

                var result = new Dictionary<Point, int>();
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    for (int x = 0; x < map.GetLength(0); x++)
                    {
                        var value = lenghts[x, y];
                        if (value.HasValue && value.Value > 0)
                            result.Add(new Point(x, y), value.Value);
                    }
                }

                return result;
            }

            public void Print()
            {
                string[,] data = new string[map.GetLength(0), map.GetLength(1)];
                for (int y = 0; y < map.GetLength(1); y++)
                    for (int x = 0; x < map.GetLength(0); x++)
                        data[x, y] = map[x, y] ? "#" : ".";

                foreach (var unit in units)
                {
                    data[unit.Position.X, unit.Position.Y] = unit.Type == Day15UnitType.Elf ? "E" : "G";
                }

                for (int y = 0; y < map.GetLength(1); y++)
                {
                    for (int x = 0; x < map.GetLength(0); x++)
                        Console.Write(data[x, y]);
                    Console.WriteLine();
                }

            }
        }

        class Day15Unit
        {
            public
                int hitPoints = 200;
            readonly int attackPower = 3;
            public Day15UnitType Type { get; }

            public Point Position { get; private set; }

            public Day15Unit(Day15UnitType type, Point initialPosition, int attackPower)
            {
                Type = type;
                Position = initialPosition;
                this.attackPower = attackPower;
            }

            bool IsTarget(Day15UnitType otherUnit) => otherUnit != Type;

            public bool IsDead => hitPoints <= 0;

            static readonly List<Point> offsets = new List<Point>() { new Point(-1, 0), new Point(1, 0), new Point(0, 1), new Point(0, -1) };

            public bool TakeATurn(Day15Map map)
            {
                var allEnemyUnits = map.AllUnits.Where(x => x.IsTarget(Type)).ToList();
                if (!allEnemyUnits.Any())
                    return false;

                var costs = map.GetCosts(Position);

                (Point point, int cost)? GetCost(Day15Unit unit)
                {
                    List<(Point point, int cost)> reachablePoints = new List<(Point point, int cost)>();
                    foreach (var offset in offsets)
                    {
                        var copy = unit.Position;
                        copy.Offset(offset);
                        if (costs.ContainsKey(copy))
                        {
                            var value = costs[copy];
                            if (value > 0)
                                reachablePoints.Add((copy, value));
                        }
                    }

                    if (reachablePoints.Any())
                        return reachablePoints.OrderBy(x => x.cost).ThenBy(x => x.point.Y).ThenBy(x => x.point.X).First();
                    return null;
                }

                List<Day15Unit> possibleAttackUnits = new List<Day15Unit>();
                foreach (var offset in offsets)
                {
                    var copy = Position;
                    copy.Offset(offset);
                    var enemy = allEnemyUnits.FirstOrDefault(x => x.Position.Equals(copy));
                    if (enemy != null && !enemy.IsDead)
                        possibleAttackUnits.Add(enemy);
                }
                var unitToAttack = possibleAttackUnits.OrderBy(x => x.hitPoints).ThenBy(x => x.Position.Y).ThenBy(x => x.Position.X).FirstOrDefault();
                if (unitToAttack == null)
                {

                    var pointToReach = allEnemyUnits.Select(GetCost).Where(x => x != null).OrderBy(x => x.Value.cost).ThenBy(x => x.Value.point.Y).ThenBy(x => x.Value.point.X).FirstOrDefault();
                    if (pointToReach != null)
                    {
                        var newPoint = map.GetPath(Position, pointToReach.Value.point);
                        if (newPoint == null)
                            throw new Exception();
                        if (newPoint.Value.cost != pointToReach.Value.cost)
                            throw new Exception();

                        Position = newPoint.Value.firstStep;
                    }
                    else
                    {

                    }
                }


                //var possibleTargets = map.GetAllTargets().ToList();
                //if (possibleTargets.Where(x => x.unit.IsTarget(Type)).Count() == 0)
                //    return false;

                //var possible = possibleTargets.Select(x => (x, map.GetPaths(Position, x.unit.Position))).Where(x => x.Item2 != null).OrderBy(x => x.Item2.Value.lenght).ThenBy(x => x.Item2.Value.firstStep.Y).ThenBy(x => x.Item2.Value.firstStep.X);
                //if (!possible.Where(x => x.Item1.unit.IsTarget(Type)).Any())
                //    return true;

                //var f = possible.Where(x => x.Item1.unit.IsTarget(Type) && x.Item2.Value.lenght < 1000);
                //if (f.Any() && f.Min(x => x.Item2.Value.lenght) > 1)
                //{
                //    var actual = f.Where(x => !possibleTargets.Select(y => y.unit.Position).Contains(x.Item2.Value.firstStep)).SkipWhile(x => !x.Item1.canBeTargeted).FirstOrDefault();
                //    if (actual.Item2 != null)
                //        Position = actual.Item2.Value.firstStep;
                //}

                //attack


                //List<Day15Unit> possibleAttackUnits = new List<Day15Unit>();
                foreach (var offset in offsets)
                {
                    var copy = Position;
                    copy.Offset(offset);
                    var enemy = allEnemyUnits.FirstOrDefault(x => x.Position.Equals(copy));
                    if (enemy != null && !enemy.IsDead)
                        possibleAttackUnits.Add(enemy);
                }
                unitToAttack = possibleAttackUnits.OrderBy(x => x.hitPoints).ThenBy(x => x.Position.Y).ThenBy(x => x.Position.X).FirstOrDefault();
                if (unitToAttack != null)
                    unitToAttack.hitPoints -= attackPower;

                return true;
            }



        }

        class Day15Point
        {
            public int X { get; }
            public int Y { get; }

            public Day15Unit Unit { get; set; }

            public bool CanPass => Unit == null;

            public Day15Point(int x, int y)
            {
                X = x;
                Y = y;
            }

            public List<Day15Point> Neighbours { get; } = new List<Day15Point>();


        }


        enum Day15UnitType
        {
            Elf, Goblin
        }



        #endregion
    }
}

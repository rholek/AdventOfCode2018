using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace AdventOfCode
{
    public class Day13
    {
        public static void Run()
        {
            var lines = File.ReadAllLines(@"Day13\Input.txt");
            Day13TrackType[,] map = new Day13TrackType[lines.Max(x => x.Length), lines.Length];
            List<Day13Train> trains = new List<Day13Train>();
            for (var y = 0; y < lines.Length; y++)
            {
                var line = lines[y];
                for (int x = 0; x < line.Length; x++)
                {
                    switch (line[x])
                    {
                        case '|':
                            map[x, y] = Day13TrackType.UpDown;
                            break;
                        case '-':
                            map[x, y] = Day13TrackType.LeftRight;
                            break;
                        case '+':
                            map[x, y] = Day13TrackType.All;
                            break;
                        case '/':
                            map[x, y] = Day13TrackType.Right;
                            break;
                        case '\\':
                            map[x, y] = Day13TrackType.Left;
                            break;
                        case '>':
                            map[x, y] = Day13TrackType.LeftRight;
                            trains.Add(new Day13Train(Day13Direction.Right, new Point(x, y)));
                            break;
                        case '<':
                            map[x, y] = Day13TrackType.LeftRight;
                            trains.Add(new Day13Train(Day13Direction.Left, new Point(x, y)));
                            break;
                        case '^':
                            map[x, y] = Day13TrackType.UpDown;
                            trains.Add(new Day13Train(Day13Direction.Up, new Point(x, y)));
                            break;
                        case 'v':
                            map[x, y] = Day13TrackType.UpDown;
                            trains.Add(new Day13Train(Day13Direction.Down, new Point(x, y)));
                            break;
                    }
                }
            }

            foreach (var day13Train in trains)
            {
                day13Train.Map = map;
            }

            while (true)
            {
                trains.Sort(Comparer<Day13Train>.Create((a, b) =>
                {
                    if (a.Position.X == b.Position.X)
                        return a.Position.Y - b.Position.Y;
                    return a.Position.X - b.Position.X;
                }));

                for (var index = 0; index < trains.Count; index++)
                {
                    var day13Train = trains[index];
                    day13Train.Move();
                    if (RemovedCrushedTrains())
                        index--;
                }

                if (trains.Count == 1)
                    break;

                bool RemovedCrushedTrains()
                {
                    var crushedTrains = trains.GroupBy(x => x.Position).Where(x => x.Count() > 1).SelectMany(x => x).ToList();
                    trains.RemoveAll(x => crushedTrains.Contains(x));
                    return crushedTrains.Any();
                }
            }

            Console.WriteLine($"{trains[0].Position.X},{trains[0].Position.Y}");
        }


        class Day13Train
        {
            #region Constants

            static readonly int[] DIRECTION_CHANGE = { -1, 0, 1 };

            #endregion

            #region Fields

            private Day13Direction currentDirection;
            private int currentPositionInDirectionChange;

            #endregion

            #region Properties

            public Point Position { get; private set; }

            public Day13TrackType[,] Map { get; set; }

            #endregion

            #region Constructor

            public Day13Train(Day13Direction direction, Point initialPosition)
            {
                currentDirection = direction;
                Position = initialPosition;
            }

            #endregion

            #region Methods

            public void Move()
            {
                //move
                switch (currentDirection)
                {
                    case Day13Direction.Left:
                        Position = new Point(Position.X - 1, Position.Y);
                        break;
                    case Day13Direction.Up:
                        Position = new Point(Position.X, Position.Y - 1);
                        break;
                    case Day13Direction.Right:
                        Position = new Point(Position.X + 1, Position.Y);
                        break;
                    case Day13Direction.Down:
                        Position = new Point(Position.X, Position.Y + 1);
                        break;
                }

                //rotate
                Day13TrackType currentTrack = Map[Position.X, Position.Y];
                switch (currentTrack)
                {
                    case Day13TrackType.None:
                        throw new Exception("Error! Train out of track!!");
                    case Day13TrackType.UpDown:
                        break;
                    case Day13TrackType.LeftRight:
                        break;
                    case Day13TrackType.Left:
                        switch (currentDirection)
                        {
                            case Day13Direction.Left:
                                currentDirection = Day13Direction.Up;
                                break;
                            case Day13Direction.Up:
                                currentDirection = Day13Direction.Left;
                                break;
                            case Day13Direction.Right:
                                currentDirection = Day13Direction.Down;
                                break;
                            case Day13Direction.Down:
                                currentDirection = Day13Direction.Right;
                                break;
                        }
                        break;
                    case Day13TrackType.Right:
                        switch (currentDirection)
                        {
                            case Day13Direction.Left:
                                currentDirection = Day13Direction.Down;
                                break;
                            case Day13Direction.Up:
                                currentDirection = Day13Direction.Right;
                                break;
                            case Day13Direction.Right:
                                currentDirection = Day13Direction.Up;
                                break;
                            case Day13Direction.Down:
                                currentDirection = Day13Direction.Left;
                                break;
                        }
                        break;
                    case Day13TrackType.All:
                        currentDirection = (Day13Direction)(((int)currentDirection + DIRECTION_CHANGE[currentPositionInDirectionChange] + 4) % 4);
                        currentPositionInDirectionChange = (currentPositionInDirectionChange + 1) % DIRECTION_CHANGE.Length;
                        break;
                }
            }

            #endregion
        }

        enum Day13Direction
        {
            Left = 0, Up = 1, Right = 2, Down = 3
        }

        enum Day13TrackType
        {
            None, UpDown, LeftRight, Left, Right, All
        }

    }
}

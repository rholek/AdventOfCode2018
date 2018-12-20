using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode
{
    public class Day20
    {
        public static void Run()
        {
            var startRoom = CreateMap(File.ReadAllText(@"Day20\Input.txt"));
            var allRooms = startRoom.AllRooms.ToList();
            Console.WriteLine($"Part 1 : {allRooms.Max(x => x.DistanceFromStart)}");
            Console.WriteLine($"Part 2 : {allRooms.Count(x => x.DistanceFromStart >= 1000)}");
        }

        public static Day20Room CreateMap(string input)
        {
            input = new string(input.SkipWhile(x => x.Equals('^')).TakeWhile(x => x != '$').ToArray());
            var startRoom = new Day20Room();
            Process(input, new List<Day20Room> { startRoom });
            startRoom.CalculateDistances();
            return startRoom;

            List<Day20Room> Process(string a, List<Day20Room> inputRooms)
            {
                List<Day20Room> resultingRooms = new List<Day20Room>();
                List<Day20Room> currentRooms = inputRooms.ToList();
                for (int i = 0; i < a.Length; i++)
                {
                    var character = a[i];
                    switch (character)
                    {
                        case '(':
                            int count = 1;
                            var sb = new StringBuilder();
                            while (count != 0)
                            {
                                i++;
                                if (a[i] == '(')
                                    count++;
                                else if (a[i] == ')')
                                    count--;
                                sb.Append(a[i]);
                            }

                            resultingRooms.Clear();
                            var next = sb.ToString();
                            currentRooms = Process(next.Substring(0, next.Length - 1), currentRooms);
                            break;
                        case '|':
                            resultingRooms.AddRange(currentRooms);
                            currentRooms = inputRooms.ToList();
                            break;
                        case ')':
                            throw new Exception("Unexceptions input. Maybe mismatched parentheses?");
                        default:
                            currentRooms = currentRooms.Select(x => x.GetRoom(character)).ToList();
                            break;
                    }
                }
                return resultingRooms.Distinct().ToList();
            }
        }
    }

    public class Day20Room
    {
        #region Fields

        private Day20Room north;
        private Day20Room west;
        private Day20Room east;
        private Day20Room south;

        #endregion

        #region Properties

        public int DistanceFromStart { get; private set; } = int.MaxValue;

        #endregion

        #region Methods

        public Day20Room GetRoom(char direction)
        {
            switch (direction)
            {
                case 'W':
                    return west ?? (west = new Day20Room { east = this });
                case 'E':
                    return east ?? (east = new Day20Room { west = this });
                case 'N':
                    return north ?? (north = new Day20Room { south = this });
                case 'S':
                    return south ?? (south = new Day20Room { north = this });
            }

            throw new Exception("Unkonwn direction!");
        }

        #region All rooms enumeration

        public IEnumerable<Day20Room> AllRooms => GetAllRooms(new List<Day20Room>());

        private IEnumerable<Day20Room> GetAllRooms(List<Day20Room> retention)
        {
            var result = new List<Day20Room>();
            if (retention.Contains(this))
                return result;
            retention.AddUnique(this);
            if (north != null)
                result.AddRange(north.GetAllRooms(retention));
            if (south != null)
                result.AddRange(south.GetAllRooms(retention));
            if (west != null)
                result.AddRange(west.GetAllRooms(retention));
            if (east != null)
                result.AddRange(east.GetAllRooms(retention));
            result.Add(this);
            return result.Distinct();
        }

        #endregion

        #region Distance calculation

        public void CalculateDistances()
        {
            CalculateDistance(0);
        }

        private void CalculateDistance(int newDistance)
        {
            if (newDistance >= DistanceFromStart)
                return;

            DistanceFromStart = newDistance;
            north?.CalculateDistance(DistanceFromStart + 1);
            east?.CalculateDistance(DistanceFromStart + 1);
            west?.CalculateDistance(DistanceFromStart + 1);
            south?.CalculateDistance(DistanceFromStart + 1);
        }

        #endregion

        #endregion
    }
}

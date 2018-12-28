using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode
{
    public class Day25
    {
        public static void Run()
        {
            List<SpaceTimeCoordinates> allCoordinates = new List<SpaceTimeCoordinates>();
            foreach (var line in File.ReadAllLines(@"Day25\Input.txt"))
            {
                var data = line.Split(',').Select(long.Parse).ToList();
                allCoordinates.Add(new SpaceTimeCoordinates(data[0], data[1], data[2], data[3]));
            }

            List<List<SpaceTimeCoordinates>> constaletaions = new List<List<SpaceTimeCoordinates>>();
            foreach (var current in allCoordinates)
            {
                var possibleConstalations = constaletaions.Where(x => x.Any(y => y.GetDistance(current) <= 3)).ToList();
                if(possibleConstalations.Count == 0)
                {
                    constaletaions.Add(new List<SpaceTimeCoordinates> { current });
                }
                else if(possibleConstalations.Count == 1)
                {
                    possibleConstalations[0].Add(current);
                }
                else
                {
                    var final = possibleConstalations[0];
                    final.Add(current);
                    foreach (var item in possibleConstalations.Skip(1))
                    {
                        final.AddRange(item);
                        constaletaions.Remove(item);
                    }
                }
            }

            Console.WriteLine(constaletaions.Count);
        }

        class SpaceTimeCoordinates
        {

            public long X { get; }
            public long Y { get; }
            public long Z { get; }
            public long T { get; }

            public SpaceTimeCoordinates(long x, long y, long z, long t)
            {
                X = x;
                Y = y;
                Z = z;
                T = t;
            }

            public override int GetHashCode()
            {
                return (int)(X ^ Y ^ Z ^ T);
            }

            public override bool Equals(object obj)
            {
                if (obj is SpaceTimeCoordinates other)
                    return X == other.X && Y == other.Y && Z == other.Z && T == other.T;
                return false;
            }

       

            public long GetDistance(SpaceTimeCoordinates other)
            {
                long distance = 0;
                distance += Math.Abs(X - other.X);
                distance += Math.Abs(Y - other.Y);
                distance += Math.Abs(Z - other.Z);
                distance += Math.Abs(T - other.T);

                return distance;
            }
          
        }

    }
}

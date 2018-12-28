using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode
{
    public class Day23
    {
        public static void Run()
        {
            List<(Coordinates pos, long r)> nanobots = new List<(Coordinates pos, long r)>();
            foreach (var line in File.ReadAllLines(@"Day23\Input.txt"))
            {
                var strings = line.Split(' ');
                var posStirng = strings[0].Substring(5, strings[0].Length - 7);
                var x = long.Parse(posStirng.Split(',')[0]);
                var y = long.Parse(posStirng.Split(',')[1]);
                var z = long.Parse(posStirng.Split(',')[2]);
                var r = long.Parse(strings[1].Split('=')[1]);

                nanobots.Add((new Coordinates(x, y, z), r));
            }

            var strongestNanobot = nanobots.OrderByDescending(x => x.r).First();
            Console.WriteLine($"Part 1: {nanobots.Count(x => x.pos.GetDistance(strongestNanobot.pos) <= strongestNanobot.r)}");

            long minX = nanobots.Min(x => x.pos.X - x.r);
            long minY = nanobots.Min(x => x.pos.Y - x.r);
            long minZ = nanobots.Min(x => x.pos.Z - x.r);
            long maxX = nanobots.Max(x => x.pos.X + x.r);
            long maxY = nanobots.Max(x => x.pos.Y + x.r);
            long maxZ = nanobots.Max(x => x.pos.Z + x.r);
            var size = Math.Max(maxX - minX, Math.Max(maxY - minY, maxZ - minZ));
            //make size even
            if (size % 2 != 0)
                size += 1;
            List<Block> blocks = new List<Block>() { new Block(new Coordinates(maxX, maxY, maxZ), size) };
            blocks[0].SetNanobotsInRange(nanobots);
            while (true)
            {
                var blockToProcess = blocks.OrderByDescending(x => x.NumberOfNanobotsInRange).ThenBy(x => x.DistanceToStart).ThenBy(x => x.Size).First();
                if (blockToProcess.Size == 1)
                {
                    Console.WriteLine($"Part 2: {blockToProcess.DistanceToStart} with {blockToProcess.NumberOfNanobotsInRange} nanobots.");
                    return;
                }

                var newblocks = blockToProcess.Split().ToList();
                foreach (var item in newblocks)
                    item.SetNanobotsInRange(nanobots);
                blocks.Remove(blockToProcess);
                blocks.AddRange(newblocks);
            }
        }

        class Block
        {

            Coordinates startCoordinates;

            IEnumerable<Coordinates> Vertecies
            {
                get
                {
                    long offset = -Size + 1;
                    yield return startCoordinates;
                    yield return startCoordinates.Offset(-Size + 1, 0, 0);
                    yield return startCoordinates.Offset(0, offset, 0);
                    yield return startCoordinates.Offset(0, 0, offset);
                    yield return startCoordinates.Offset(offset, offset, 0);
                    yield return startCoordinates.Offset(offset, 0, offset);
                    yield return startCoordinates.Offset(0, offset, offset);
                    yield return startCoordinates.Offset(offset, offset, offset);
                }
            }

            public long Size { get; }

            public int NumberOfNanobotsInRange { get; private set; }

            public long DistanceToStart => Vertecies.Min(x => x.GetDistanceFromStart());

            public Block(Coordinates startCoordinates, long size)
            {
                this.startCoordinates = startCoordinates;
                Size = size;
            }

            public IEnumerable<Block> Split()
            {
                long newSize = Size / 2;
                yield return new Block(startCoordinates, newSize);
                yield return new Block(startCoordinates.Offset(-newSize, 0, 0), newSize);
                yield return new Block(startCoordinates.Offset(0, -newSize, 0), newSize);
                yield return new Block(startCoordinates.Offset(0, 0, -newSize), newSize);
                yield return new Block(startCoordinates.Offset(-newSize, 0, 0), newSize);
                yield return new Block(startCoordinates.Offset(-newSize, -newSize, 0), newSize);
                yield return new Block(startCoordinates.Offset(-newSize, 0, -newSize), newSize);
                yield return new Block(startCoordinates.Offset(0, -newSize, -newSize), newSize);
                yield return new Block(startCoordinates.Offset(-newSize, -newSize, -newSize), newSize);
            }

      

            public void SetNanobotsInRange(List<(Coordinates pos, long r)> nanobots)
            {
                List<(Coordinates pos, long r)> nanobotsInRange = new List<(Coordinates pos, long r)>();
                foreach (var vertex in Vertecies)
                {
                    nanobotsInRange.AddRange(nanobots.Where(x => x.pos.GetDistance(vertex) <= x.r));
                }
                nanobotsInRange = nanobotsInRange.Distinct().ToList();
                foreach (var nanobot in nanobots.Except(nanobotsInRange).ToList())
                {
                    foreach (var item in GetRangeBoundaries(nanobot))
                    {
                        if (item.X <= startCoordinates.X && item.X > startCoordinates.X - Size
                            && item.Y <= startCoordinates.Y && item.Y > startCoordinates.Y - Size
                            && item.Z <= startCoordinates.Z && item.Z > startCoordinates.Z - Size)
                        {
                            nanobotsInRange.Add(nanobot);
                            break;
                        }
                    }
                }

                NumberOfNanobotsInRange = nanobotsInRange.Distinct().Count();

                IEnumerable<Coordinates> GetRangeBoundaries((Coordinates pos, long r) nanobot)
                {
                    yield return nanobot.pos.Offset(nanobot.r, 0, 0);
                    yield return nanobot.pos.Offset(-nanobot.r, 0, 0);
                    yield return nanobot.pos.Offset(0, nanobot.r, 0);
                    yield return nanobot.pos.Offset(0, -nanobot.r, 0);
                    yield return nanobot.pos.Offset(0, 0, nanobot.r);
                    yield return nanobot.pos.Offset(0, 0, -nanobot.r);
                }
            }
        }
        
        class Coordinates
        {
            public static readonly Coordinates Empty = new Coordinates(0, 0, 0);

            public long X { get; }
            public long Y { get; }
            public long Z { get; }

            public Coordinates(long x, long y, long z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public override int GetHashCode()
            {
                return (int)(X ^ Y ^ Z);
            }

            public override bool Equals(object obj)
            {
                if (obj is Coordinates other)
                    return X == other.X && Y == other.Y && Z == other.Z;
                return false;
            }

            public long GetDistanceFromStart()
            {
                return GetDistance(Coordinates.Empty);
            }

            public long GetDistance(Coordinates other)
            {
                long distance = 0;
                distance += Math.Abs(X - other.X);
                distance += Math.Abs(Y - other.Y);
                distance += Math.Abs(Z - other.Z);
                return distance;
            }

            public Coordinates Offset(long x, long y, long z)
            {
                return new Coordinates(X + x, Y + y, Z + z);
            }
        }

    }
}

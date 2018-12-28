using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace AdventOfCode
{
    public class Day22
    {
        const bool SHOW_PROGRESS = true;

        //TestInput
        //const int DEPTH = 510;
        //static readonly Point Target = new Point(10, 10);

        //Input
        const long DEPTH = 4080;
        static readonly Point Target = new Point(14, 785);

        public static void Run()
        {
            Map map = new Map();
            Console.WriteLine($"Part 1: {map.GetTotalDanger(Target)}");

            var currentLowestPrice = map.GetRegion(new Point(0, 0), Day22Equipment.Torch);
            currentLowestPrice.Price = 0;
            currentLowestPrice.Visited = true;

            int counter = 0;
            HashSet<Region> openList = new HashSet<Region>(currentLowestPrice.NeighboringRegions);
            while (currentLowestPrice != null)
            {
                currentLowestPrice = openList.OrderBy(x => x.PredictedPrice).FirstOrDefault();
                currentLowestPrice.Visit();
                openList.Remove(currentLowestPrice);
                foreach (var r in currentLowestPrice.NeighboringRegions.Where(x => !x.Visited))
                    openList.AddUnique(r);

                if (currentLowestPrice.point.Equals(Target) && currentLowestPrice.Equipment == Day22Equipment.Torch)
                    break;

                counter++;
                if (SHOW_PROGRESS && counter % 10000 == 0)
                {
                    //show some progress, because it can take a while
                    var percent = (double)(currentLowestPrice.point.X + currentLowestPrice.point.Y) / (Target.X + Target.Y);
                    Console.WriteLine($"Cost {currentLowestPrice.Price} to {currentLowestPrice.point}. " +
                        $"Approx {Math.Round(percent, 2) * 100}% of the journey.");
                }
            }

            Console.WriteLine($"Part 2: {map.GetRegion(Target, Day22Equipment.Torch).Price}");
        }

        class Map
        {
            private Dictionary<Point, Dictionary<Day22Equipment, Region>> allRegions;

            public Map()
            {
                allRegions = new Dictionary<Point, Dictionary<Day22Equipment, Region>>();
                //cap search area at Taget + 200 (should be enough)
                for (int y = 0; y <= Target.Y + 200; y++)
                {
                    for (int x = 0; x <= Target.X + 200; x++)
                    {
                        var point = new Point(x, y);
                        var r = new Region(point, this);
                        var erosionIndex = r.GetErosionIndex();
                        allRegions[point] = new Dictionary<Day22Equipment, Region>();

                        foreach (var item in r.Type.GetPossibleEquipments().GetAllItems())
                        {
                            allRegions[point][item] = new Region(point, this, erosionIndex, item);
                        }
                    }
                }
            }

            /// <summary>
            /// Get total danger up to spcified point
            /// </summary>
            public long GetTotalDanger(Point to)
            {
                long result = 0;
                for (int y = 0; y <= to.Y; y++)
                {
                    for (int x = 0; x <= to.X; x++)
                    {
                        result += allRegions[new Point(x, y)].First().Value.GetErosionIndex() % 3;
                    }
                }

                return result;
            }

            /// <summary>
            /// Get region at coordinates with specific equipment
            /// </summary>
            public Region GetRegion(Point point, Day22Equipment eq)
            {
                return allRegions.GetValueOrDefault(point)?.GetValueOrDefault(eq);
            }

            /// <summary>
            /// Get region at coordinates with any equipment
            /// </summary>
            public Region GetRegion(Point point)
            {
                return allRegions.GetValueOrDefault(point).First().Value;
            }
        }

        class Region
        {
            #region Fields

            private long? erosionIndex;
            public Point point;
            Map map;

            #endregion

            #region Constructors


            public Region(Point point, Map map)
            {
                this.point = point;
                this.map = map;
            }

            public Region(Point point, Map map, Day22Equipment equipment) : this(point, map)
            {
                this.Equipment = equipment;
            }

            public Region(Point point, Map map, long erosionIndex, Day22Equipment equipment) : this(point, map, equipment)
            {
                this.erosionIndex = erosionIndex;
            }

            #endregion

            #region Properties

            public Day22Equipment Equipment { get; }
            public bool Visited { get; set; }
            public int Price { get; set; } = int.MaxValue;
            public long DangerLevel => GetErosionIndex() % 3;

            /// <summary>
            /// Lowest possible price with currently visited regions
            /// </summary>
            public int PredictedPrice
            {
                get
                {
                    if (Visited)
                        throw new Exception("Cannot predict price on visited region! Use Price instead.");
                    var sourceRegions = NeighboringRegions.Where(x => x.Visited);
                    if (sourceRegions == null || !sourceRegions.Any())
                        return int.MaxValue;
                    var min = sourceRegions.Min(x => PredictPrice(x));
                    return min;
                }
            }

            public Day22RegionType Type
            {
                get
                {
                    switch (DangerLevel)
                    {
                        case 0:
                            return Day22RegionType.Rocky;
                        case 1:
                            return Day22RegionType.Wet;
                        case 2:
                            return Day22RegionType.Narrow;
                    }

                    throw new Exception("Unexpected danger level.");
                }
            }

            List<Region> neighboringRegionsCache;
            public List<Region> NeighboringRegions
            {
                get
                {
                    if (neighboringRegionsCache == null)
                        neighboringRegionsCache = GetNeighboringRegions().ToList();
                    return neighboringRegionsCache;
                }
            }

            #endregion

            #region Public methods

            public long GetErosionIndex()
            {
                if (erosionIndex != null)
                    return erosionIndex.Value;
                if (point.Equals(new Point(0, 0)) || point.Equals(Target))
                    return DEPTH % 20183;
                if (point.Y == 0)
                    return ((point.X * 16807) + DEPTH) % 20183;
                if (point.X == 0)
                    return ((point.Y * 48271) + DEPTH) % 20183;
                erosionIndex = (map.GetRegion(new Point(point.X - 1, point.Y)).GetErosionIndex() *
                    map.GetRegion(new Point(point.X, point.Y - 1)).GetErosionIndex() + DEPTH) % 20183;
                return erosionIndex.Value;
            }

            public void Visit()
            {
                var sourceRegions = NeighboringRegions.Where(x => x.Visited);
                if (sourceRegions == null || !sourceRegions.Any())
                    throw new Exception("No neighboring is visited.");
                var result = sourceRegions.Select(PredictPrice).ToList();
                int price = result.Min(x => x);
                Visited = true;
                Price = price;
            }

            #endregion

            #region Private methods

            private IEnumerable<Region> GetNeighboringRegions()
            {
                foreach (var item in Type.GetPossibleEquipments().GetAllItems())
                {
                    if (item == Equipment)
                        continue;

                    var oth = map.GetRegion(new Point(point.X, point.Y), item);
                    if (oth == null)
                        throw new Exception();
                    yield return oth;
                }

                var east = map.GetRegion(new Point(point.X + 1, point.Y), Equipment);
                if (east != null)
                    yield return east;
                var south = map.GetRegion(new Point(point.X, point.Y + 1), Equipment);
                if (south != null)
                    yield return south;
                var north = map.GetRegion(new Point(point.X, point.Y - 1), Equipment);
                if (north != null)
                    yield return north;
                var west = map.GetRegion(new Point(point.X - 1, point.Y), Equipment);
                if (west != null)
                    yield return west;
            }

            int PredictPrice(Region sourceRegion)
            {
                if (!sourceRegion.Visited)
                    throw new Exception("Cannot predict price from unvisted region.");

                int price = 1;
                if (sourceRegion.Equipment != Equipment)
                {
                    if (!sourceRegion.point.Equals(point))
                        throw new Exception("Cannot both move and change equipment.");
                    price = 7;
                }
                if (sourceRegion.Equipment == Equipment)
                {
                    if (sourceRegion.point.Equals(point))
                        throw new Exception("Cannot both move and change equipment.");
                    price = 1;
                }

                return price + sourceRegion.Price;
            }

            #endregion
        }
    }

    #region Enums

    [Flags]
    enum Day22Equipment
    {
        Nothing = 1,
        Torch = 2,
        ClimbingGear = 4
    }

    enum Day22RegionType
    {
        Rocky = 0,
        Wet = 1,
        Narrow = 2
    }

    static class Day22RegionTypeExtensions
    {
        public static Day22Equipment GetPossibleEquipments(this Day22RegionType type)
        {
            switch (type)
            {
                case Day22RegionType.Rocky:
                    return Day22Equipment.ClimbingGear | Day22Equipment.Torch;
                case Day22RegionType.Wet:
                    return Day22Equipment.ClimbingGear | Day22Equipment.Nothing;
                case Day22RegionType.Narrow:
                    return Day22Equipment.Nothing | Day22Equipment.Torch;
            }

            throw new Exception();
        }

        public static string Draw(this Day22RegionType type)
        {
            switch (type)
            {
                case Day22RegionType.Rocky:
                    return ".";
                case Day22RegionType.Wet:
                    return "=";
                case Day22RegionType.Narrow:
                    return "|";
            }

            throw new Exception();
        }

        public static IEnumerable<Day22Equipment> GetAllItems(this Day22Equipment eq)
        {
            List<string> result = new List<string>();
            foreach (Day22Equipment r in Enum.GetValues(typeof(Day22Equipment)))
            {
                if ((eq & r) != 0)
                    yield return r;
            }
        }
    }

    #endregion
}
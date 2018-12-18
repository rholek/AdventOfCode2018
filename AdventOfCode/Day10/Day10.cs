using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AdventOfCode
{
    public class Day10
    {
        public static void Run()
        {
            List<Day10Point> points = new List<Day10Point>();
            foreach (var item in System.IO.File.ReadAllLines(@"Day10\Input.txt"))
            {
                var position = string.Concat(item.SkipWhile(x => x != '<').Skip(1).TakeWhile(x => x != '>'));
                var positionSplit = position.Split(',');
                var point = new Point(int.Parse(positionSplit[0].Trim()), int.Parse(positionSplit[1].Trim()));

                var speed = string.Concat(item.SkipWhile(x => x != '<').Skip(1).SkipWhile(x => x != '<').Skip(1).TakeWhile(x => x != '>'));
                var speedSplit = speed.Split(',');
                points.Add(new Day10Point(point, int.Parse(speedSplit[0].Trim()), int.Parse(speedSplit[1].Trim())));
            }

            int seconds = 0;
            int minHeight = int.MaxValue;
            int minWidth = int.MaxValue;
            while (true)
            {
                foreach (var item in points)
                    item.Move();

                int minX = points.Min(x => x.Position.X);
                int minY = points.Min(x => x.Position.Y);
                int maxX = points.Max(x => x.Position.X);
                int maxY = points.Max(x => x.Position.Y);

                var height = maxY - minY;
                var width = maxX - minX;

                if (height > minHeight && width > minWidth)
                {
                    foreach (var item in points)
                        item.MoveBack();
                    Draw();
                    break;
                }

                minHeight = height;
                minWidth = width;
                seconds++;
            }

            void Draw()
            {
                int minX = points.Min(x => x.Position.X);
                int minY = points.Min(x => x.Position.Y);
                int maxX = points.Max(x => x.Position.X);
                int maxY = points.Max(x => x.Position.Y);

                maxX = maxX + -minX + 1;
                maxY = maxY + -minY + 1;
                bool[,] display = new bool[maxX, maxY];

                foreach (var item in points)
                {
                    display[item.Position.X - minX, item.Position.Y - minY] = true;
                }

                Console.Clear();
                Console.WriteLine($"{seconds}s");
                for (int y = 0; y < maxY; y++)
                {
                    for (int x = 0; x < maxX; x++)
                    {
                        if (display[x, y])
                            Console.Write("X");
                        else
                            Console.Write("-");
                    }
                    Console.WriteLine();
                }
            }
        }

        class Day10Point
        {
            public Point Position { get; private set; }

            Size speed;

            public Day10Point(Point startPosition, int speedX, int speedY)
            {
                Position = startPosition;
                speed = new Size(speedX, speedY);
            }

            public void Move()
            {
                Position = Position + speed;
            }

            public void MoveBack()
            {
                Position = Position - speed;
            }

        }
    }
}

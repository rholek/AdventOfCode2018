using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AdventOfCode
{
    public class Day11
    {
        public static void Run()
        {
            const int serialId = 3999;
            const int SIZE = 300;

            int[,] cache = new int[SIZE, SIZE];

            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                {
                    cache[i, j] = GetCellValue(i + 1, j + 1);
                }
            }

            (int x, int y, int size, long value) max = (-1, -1, 0, 0);

            for (int xx = 1; xx <= SIZE; xx++)
            {
                for (int y = 1; y <= SIZE; y++)
                {
                    for (int size = 1; size <= Math.Min(SIZE - xx, SIZE - y) + 1; size++)
                    {
                        var value = GetAreaValue(xx, y, size);
                        if (max.value < value)
                            max = (xx, y, size, value);
                    }
                }
            }

            Console.WriteLine($"{max.x},{max.y},{max.size}");

            long GetAreaValue(int x, int y, int size)
            {
                long sum = 0;
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        sum += cache[x + i - 1, y + j - 1];
                    }
                }

                return sum;
            }


            int GetCellValue(int x, int y)
            {
                var rack = x + 10;
                var result = rack * y;
                result = result + serialId;
                result = result * rack;
                result = (result / 100) % 10;
                result = result - 5;
                return result;
            }
        }
    }
}

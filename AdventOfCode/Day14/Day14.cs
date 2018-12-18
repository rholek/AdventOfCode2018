using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode
{
    public class Day14
    {
        public static void Run()
        {
            const string patternToFind = "990941";

            List<int> scoreBoard = new List<int> { 3, 7 };
            int firstElfIndex = 0;
            int secondElfIndex = 1;
            int iteration = 0;
            while (true)
            {
                var firstElfValue = scoreBoard[firstElfIndex];
                var secondElfValue = scoreBoard[secondElfIndex];
                var totalScore = firstElfValue + secondElfValue;
                if (totalScore > 9)
                    scoreBoard.Add(totalScore / 10);
                scoreBoard.Add(totalScore % 10);

                firstElfIndex = (firstElfIndex + firstElfValue + 1) % scoreBoard.Count;
                secondElfIndex = (secondElfIndex + secondElfValue + 1) % scoreBoard.Count;
                if (iteration % 1000000 == 0)
                {
                    var sb = new StringBuilder();
                    foreach (var i in scoreBoard)
                        sb.Append(i);
                    var scoreBoardString = sb.ToString();
                    int index = scoreBoardString.IndexOf(patternToFind, StringComparison.OrdinalIgnoreCase);
                    if (index > 0)
                    {
                        Console.WriteLine(index);
                        break;
                    }
                }

                iteration++;
            }
        }


    }
}

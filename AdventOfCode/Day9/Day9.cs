using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    public class Day9
    {
        public static void Run()
        {
            const int PLAYERS = 424;
            const int MAX_MARBLE_VALUE = 71144 * 100;// remove * for part I

            List<Day9Player> players = new List<Day9Player>();
            int currentPlayer = 0;
            for (int i = 0; i < PLAYERS; i++)
            {
                players.Add(new Day9Player());
            }

            Day9Marble currentMarble = new Day9Marble(0);
            currentMarble.Next = currentMarble;
            currentMarble.Previous = currentMarble;

            for (uint currentMarbleValue = 1; currentMarbleValue <= MAX_MARBLE_VALUE; currentMarbleValue++)
            {
                var player = players[currentPlayer];
                var marble = new Day9Marble(currentMarbleValue);
                if (marble.MarbleValue % 23 == 0)
                {
                    player.AddMarble(marble);
                    var marble7 = currentMarble.GetMarble(-7);
                    player.AddMarble(marble7);

                    var marble8 = marble7.GetMarble(-1);
                    var marble6 = marble7.GetMarble(1);
                    marble8.Next = marble6;
                    marble6.Previous = marble8;
                    currentMarble = marble6;
                }
                else
                {
                    var next1 = currentMarble.GetMarble(1);
                    var next2 = currentMarble.GetMarble(2);

                    marble.Next = next2;
                    next2.Previous = marble;
                    marble.Previous = next1;
                    next1.Next = marble;
                    currentMarble = marble;
                }

                currentPlayer = (currentPlayer + 1) % PLAYERS;

            }

            Console.WriteLine(players.Max(x => x.TotalScore));
        }

        class Day9Marble
        {
            public Day9Marble Next { get; set; }

            public Day9Marble Previous { get; set; }

            public double MarbleValue { get; }

            public Day9Marble GetMarble(int relativePosition)
            {
                if (relativePosition == 0)
                    return this;

                if (relativePosition > 0)
                    return Next.GetMarble(relativePosition - 1);

                if (relativePosition < 0)
                    return Previous.GetMarble(relativePosition + 1);

                throw new Exception();
            }

            public Day9Marble(double marbleValue)
            {
                MarbleValue = marbleValue;
            }
        }

        class Day9Player
        {
            private readonly List<Day9Marble> marbeles = new List<Day9Marble>();

            public void AddMarble(Day9Marble marble)
            {
                marbeles.Add(marble);
            }

            public double TotalScore => marbeles.Sum(x => x.MarbleValue);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode
{
    public class Day12
    {
        public static void Run()
        {
                //-------------------------Init
                Dictionary<string, bool> rulesCollection = new Dictionary<string, bool>();
                foreach (var ruleInput in File.ReadAllLines(@"Day12\Rules.txt"))
                {
                    rulesCollection[ruleInput.Substring(0, 5)] = ruleInput[9] == '#';
                }

                const string initialState = ".##..##..####..#.#.#.###....#...#..#.#.#..#...#....##.#.#.#.#.#..######.##....##.###....##..#.####.#";
                Day12Game currentGeneration = new Day12Game();
                for (var index = 0; index < initialState.Length; index++)
                {
                    currentGeneration[index] = initialState[index] == '#';
                }

                //-------------------------Start
                const long GENERATIONS = 50000000000;
                long firstStableGeneration = 0;
                int movePerGeneration = 0;
                HashSet<string> visitedStates = new HashSet<string>();
                for (long i = 0; i < GENERATIONS; i++)
                {
                    var newGeneration = currentGeneration.Clone();

                    for (int j = currentGeneration.LowestIndex - 3; j <= currentGeneration.HighestIndex + 3; j++)
                        newGeneration[j] = rulesCollection[currentGeneration.GetSubState(j, 2)];
                    newGeneration.Trim();

                    if (newGeneration.State == currentGeneration.State)
                    {
                        firstStableGeneration = i;
                        movePerGeneration = newGeneration.HighestIndex - currentGeneration.HighestIndex;
                        break;
                    }

                    currentGeneration = newGeneration;
                }

                //-------------------------Show Result
                long total = 0;
                for (int j = currentGeneration.LowestIndex; j < currentGeneration.HighestIndex + 1; j++)
                {
                    if (currentGeneration[j])
                        total += j + (GENERATIONS - firstStableGeneration) * movePerGeneration;
                }
                Console.WriteLine(total);
            }

        class Day12Game
        {
            #region Fields

            readonly Dictionary<int, bool> values = new Dictionary<int, bool>();

            #endregion

            #region Constructors

            public Day12Game() { }

            private Day12Game(Dictionary<int, bool> pValues)
            {
                values = pValues;
            }

            #endregion

            #region Properties

            public string State
            {
                get
                {
                    var sb = new StringBuilder();
                    foreach (var item in values.OrderBy(x => x.Key))
                    {
                        sb.Append(item.Value ? "#" : ".");
                    }

                    return sb.ToString().Trim('.');
                }
            }

            public int LowestIndex => values.Min(x => x.Key);
            public int HighestIndex => values.Max(x => x.Key);

            public bool this[int index]
            {
                get
                {
                    if (values.ContainsKey(index))
                        return values[index];
                    return default(bool);
                }

                set
                {
                    values[index] = value;
                }
            }

            #endregion

            #region Methods

            public void Trim()
            {
                var toRemove = values.OrderBy(x => x.Key).TakeWhile(x => !x.Value).Select(x => x.Key).ToList();
                toRemove.AddRange(values.OrderByDescending(x => x.Key).TakeWhile(x => !x.Value).Select(x => x.Key).ToList());
                foreach (var item in toRemove)
                    values.Remove(item);
            }

            public string GetSubState(int middle, int range)
            {
                var sb = new StringBuilder();

                for (int i = middle - range; i <= middle + range; i++)
                {
                    sb.Append(this[i] ? "#" : ".");
                }
                return sb.ToString();
            }

            public Day12Game Clone()
            {
                return new Day12Game(values.ToDictionary(x => x.Key, x => x.Value));
            }

            #endregion
        }
    }
}

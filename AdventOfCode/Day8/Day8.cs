using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode
{
    public class Day8
    {
        public static void Run()
        {
            var data = File.ReadAllText(@"Day8\Input.txt").Split(' ');
            int currentIndex = 0;
            var root = ProcessNode();

            Console.WriteLine(root.MetadataSumIncludingChildren);
            Console.WriteLine(root.NodeValue);

            Day8Node ProcessNode()
            {
                var childCount = int.Parse(data[currentIndex++]);
                var metadataCount = int.Parse(data[currentIndex++]);
                var node = new Day8Node();

                for (int i = 0; i < childCount; i++)
                {
                    node.ChildNodes.Add(ProcessNode());
                }

                for (int i = 0; i < metadataCount; i++)
                {
                    node.Metadata.Add(int.Parse(data[currentIndex++]));
                }

                return node;
            }

        }

        class Day8Node
        {
            public List<Day8Node> ChildNodes { get; } = new List<Day8Node>();

            public List<int> Metadata { get; } = new List<int>();


            public int MetadataSumIncludingChildren => ChildNodes.Sum(x => x.MetadataSumIncludingChildren) + Metadata.Sum();

            public int NodeValue
            {
                get
                {
                    if (ChildNodes.Count == 0)
                        return Metadata.Sum();

                    int result = 0;
                    foreach (var item in Metadata)
                    {
                        int index = item - 1;
                        if (index < 0 || index > ChildNodes.Count - 1)
                            continue;

                        var childOnIndex = ChildNodes[index];
                        result += childOnIndex?.NodeValue ?? 0;
                    }

                    return result;
                }
            }

        }
    }
}

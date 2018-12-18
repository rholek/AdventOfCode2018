using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode
{
    public class Day7
    {
        public static void Run()
        {

            Dictionary<char, Day7Step> steps = new Dictionary<char, Day7Step>();
            foreach (var item in File.ReadAllLines(@"Day7\Input.txt"))
            {
                GetStep(item[36]).SourceSteps.Add(GetStep(item[5]));
            }

            //while(true)
            // {
            //     var step = steps.Values.Where(x => x.CanBeStarted).OrderBy(x => x.ID).FirstOrDefault();
            //     if (step == null)
            //         break;

            //     step./*CompleteIndex*/ = steps.Values.Max(x => x.CanBeStarted ?? -1) + 1;
            //     Console.Write(step.ID);

            // }


            const int MAX_STEPS_IN_PROGRESS = 5;

            List<Day7Step> stepsInProgress = new List<Day7Step>();


            int seconds = 0;
            while (true)
            {
                foreach (var item in stepsInProgress.ToList())
                {
                    item.RemainingTime -= 1;
                    if (item.IsCompleted)
                        stepsInProgress.Remove(item);
                }

                if (steps.Values.All(x => x.IsCompleted))
                    break;
                seconds++;

                for (int i = stepsInProgress.Count; i < MAX_STEPS_IN_PROGRESS; i++)
                {
                    var step = steps.Values.Where(x => x.CanBeStarted).Except(stepsInProgress).OrderBy(x => x.ID).FirstOrDefault();
                    if (step != null)
                        stepsInProgress.Add(step);
                }

            }

            Console.WriteLine(seconds);
            Day7Step GetStep(char id)
            {
                if (!steps.ContainsKey(id))
                    steps[id] = new Day7Step(id);
                return steps[id];
            }
        }

        class Day7Step
        {
            public char ID { get; }

            public List<Day7Step> SourceSteps { get; } = new List<Day7Step>();

            public Day7Step(char id)
            {
                ID = id;
                const string alphabet = " ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                RemainingTime = 60 + alphabet.IndexOf(ID);

            }

            public bool CanBeStarted => !IsCompleted && SourceSteps.All(x => x.IsCompleted);


            public int RemainingTime { get; set; }

            public bool IsCompleted => RemainingTime == 0;
        }
    }
}

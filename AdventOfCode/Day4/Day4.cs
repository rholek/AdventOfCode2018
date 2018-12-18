using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode
{
    public class Day4
    {
        public static void Run()
        {
            List<(DateTime cas, string akce)> data = new List<(DateTime cas, string akce)>();
            var words = File.ReadAllLines(@"Day4\Input.txt");
            foreach (var s in words)
            {
                var datum = DateTime.Parse(s.Substring(1, 16), CultureInfo.InvariantCulture);
                var akce = s.Substring(19);
                data.Add((datum, akce));


            }

            data = data.OrderBy(x => x.cas).ThenByDescending(x =>
            {
                if (x.akce.StartsWith("Guard"))
                    return 3;
                if (x.akce.StartsWith("wake"))
                    return 2;
                if (x.akce.StartsWith("fall"))
                    return 1;
                return 0;
            }).ToList();
            string currentGuard = null;
            bool asleep = false;
            Dictionary<string, int[]> guards = new Dictionary<string, int[]>();
            int fallAsleepMinute = 0;
            int wakeUpMinute = 0;
            foreach (var item in data)
            {
                var newGuard = GetGuard(item.akce);
                if (newGuard != null)
                {
                    if (!guards.ContainsKey(newGuard))
                        guards[newGuard] = new int[60];

                    currentGuard = newGuard;
                    OnWakeUp();
                    fallAsleepMinute = asleep ? 0 : 60;
                }



                if (item.akce == "falls asleep")
                {
                    asleep = true;
                    fallAsleepMinute = item.cas.Minute;
                }

                if (item.akce == "wakes up")
                {
                    wakeUpMinute = item.cas.Minute;
                    OnWakeUp();
                }

                void OnWakeUp()
                {
                    for (int i = fallAsleepMinute; i < wakeUpMinute; i++)
                    {
                        guards[currentGuard][i]++;
                    }

                    asleep = false;
                    fallAsleepMinute = 60;
                    wakeUpMinute = 0;
                }
            }

            var max = guards.Max(y => y.Value.Max());
            var guardWithMaxSpleep = guards.First(x => x.Value.Any(y => y == max));
            Console.WriteLine(guardWithMaxSpleep.Key);
            Console.WriteLine(guardWithMaxSpleep.Value.ToList().FindIndex(x => x == guardWithMaxSpleep.Value.Max()));

            Console.WriteLine(guardWithMaxSpleep.Value.ToList().FindIndex(x => x == guardWithMaxSpleep.Value.Max()) * int.Parse(guardWithMaxSpleep.Key));


            string GetGuard(string s)
            {
                var res = s.Split(' ').ElementAtOrDefault(1)?.Trim('#');
                if (int.TryParse(res, out _))
                    return res;

                return null;
            }
        }
    }
}

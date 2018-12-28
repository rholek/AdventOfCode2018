using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    public class Day24
    {
        public static void Run()
        {
            int boost = 0;
            int step = 50;
            GroupType? lastWinner = GroupType.Infection;
            while (true)
            {
                List<BattleGroup> groups = new List<BattleGroup>();
                foreach (var line in File.ReadAllLines(@"Day24\ImmuneSystem.txt"))
                    groups.Add(ParseRow(line, GroupType.ImmuneSystem));
                foreach (var line in File.ReadAllLines(@"Day24\Infection.txt"))
                    groups.Add(ParseRow(line, GroupType.Infection));

                bool draw = false;
                while (groups.GroupBy(x => x.Type).Count() > 1)
                {
                    HashSet<BattleGroup> alreadyChosenGroups = new HashSet<BattleGroup>();
                    List<(BattleGroup attacker, BattleGroup defender)> fight = new List<(BattleGroup attacker, BattleGroup defender)>();
                    foreach (var attackingGroup in groups.OrderByDescending(x => x.EffectivePower).ThenByDescending(x => x.Initiative))
                    {
                        var defendingGroup = groups.Where(x => x.GetDamage(attackingGroup) > 0 && !alreadyChosenGroups.Contains(x)).
                            OrderByDescending(x => x.GetDamage(attackingGroup)).
                            ThenByDescending(x => x.EffectivePower).
                            ThenByDescending(x => x.Initiative).
                            FirstOrDefault();

                        if (defendingGroup != null)
                        {
                            fight.Add((attackingGroup, defendingGroup));
                            alreadyChosenGroups.Add(defendingGroup);
                        }
                    }

                    var groupsBefore = groups.Select(x => x.UnitsCount).ToList();
                    foreach (var item in fight.OrderByDescending(x => x.attacker.Initiative))
                        item.defender.OnAttacked(item.attacker);

                    draw = groupsBefore.SequenceEqual(groups.Select(x => x.UnitsCount));
                    if (draw)
                        break;

                    groups.RemoveAll(x => x.IsDead);
                }

                GroupType? winner = draw ? (GroupType?)null : groups.First().Type;
                Console.WriteLine($"{(draw ? "DRAW" : winner.ToString())} ({boost}, {step}) | Total units: {groups.Sum(x => x.UnitsCount)}");
                if (step <= 1 && step >= -1 && winner == GroupType.ImmuneSystem)
                    break;

                if (!draw && lastWinner != winner)
                {
                    step *= -1;
                    step /= 2;
                }

                lastWinner = winner;
                boost += step;
            }

            Console.WriteLine("End");

            BattleGroup ParseRow(string input, GroupType type)
            {
                var splitWithoutWeakImun = Regex.Replace(input, @"\s\(.*\)", string.Empty).Split(' ');
                int unitsCount = int.Parse(splitWithoutWeakImun[0]);
                int hitPoints = int.Parse(splitWithoutWeakImun[4]);
                int attackPoints = int.Parse(splitWithoutWeakImun[12]);
                string attackType = splitWithoutWeakImun[13].Trim();
                int initiative = int.Parse(splitWithoutWeakImun[17]);
                var weakInum = Regex.Match(input, @"\(.*\)").Value.Trim("()".ToCharArray()).Split(';');
                List<string> weaknesses = new List<string>();
                List<string> imunities = new List<string>();
                foreach (var item in weakInum.Select(x => x.Trim()))
                {
                    if (item.StartsWith("weak to"))
                        weaknesses = item.Replace("weak to", "").Split(',').Select(x => x.Trim()).ToList();
                    if (item.StartsWith("immune to"))
                        imunities = item.Replace("immune to", "").Split(',').Select(x => x.Trim()).ToList();
                }

                if (type == GroupType.ImmuneSystem)
                    attackPoints += boost;

                return new BattleGroup(hitPoints, unitsCount, weaknesses, imunities, attackPoints, attackType, initiative, type);
            }

        }


        class BattleGroup
        {
            public int HitPoints { get; }
            public int UnitsCount { get; private set; }
            public List<string> Weaknesses { get; }
            public List<string> Immunities { get; }
            public int AttackDamage { get; }
            public string AttackType { get; }
            public int Initiative { get; }
            public GroupType Type { get; }
            public int EffectivePower => UnitsCount * AttackDamage;
            public bool IsDead => UnitsCount <= 0;

            public BattleGroup(int hitPoints, int unitsCount, List<string> weaknesses, List<string> immunities, int attackDamage, string attackType, int initiative, GroupType type)
            {
                HitPoints = hitPoints;
                UnitsCount = unitsCount;
                Weaknesses = weaknesses;
                Immunities = immunities;
                AttackDamage = attackDamage;
                AttackType = attackType;
                Initiative = initiative;
                Type = type;
            }

            public int GetDamage(BattleGroup attackingGroup)
            {
                if (attackingGroup.Type == Type)
                    return 0;

                int damage = attackingGroup.EffectivePower;
                if (Immunities.Contains(attackingGroup.AttackType))
                    damage = 0;
                if (Weaknesses.Contains(attackingGroup.AttackType))
                    damage *= 2;
                return damage;
            }

            public void OnAttacked(BattleGroup attackingGroup)
            {
                if (attackingGroup.Type == Type)
                    throw new Exception("WFT? I'm a friend!");
                var unitsKilled = GetDamage(attackingGroup) / HitPoints;
                UnitsCount -= unitsKilled;
                if (UnitsCount < 0)
                    UnitsCount = 0;
            }
        }

        enum GroupType
        {
            ImmuneSystem,
            Infection
        }
    }
}

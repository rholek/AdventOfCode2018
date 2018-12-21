using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode
{
    public class Day21
    {
        public static void Run()
        {
            const int IP_REGISTER = 1;

            List<(Day16Opcode operation, long[] instruction)> instructions = new List<(Day16Opcode operation, long[] instruction)>();
            foreach (var line in File.ReadAllLines(@"Day21\Input.txt"))
            {
                var strings = line.Split(' ');
                instructions.Add(((Day16Opcode)Enum.Parse(typeof(Day16Opcode), strings[0]), strings.Skip(1).Select(long.Parse).ToArray()));
            }

            List<(int startnumber, int instructionsCount)> s = new List<(int startnumber, int instructionsCount)>();
            
            //Go to all values in register 5
            //It increases its run time
            //when it repeats, maximum has been reached
            List<long> possibles = new List<long>();
            foreach (var next in GetNext())
            {
                if(possibles.Contains(next))
                    break;
                possibles.Add(next);
            }
            Console.WriteLine($"Part 1: {possibles.First()}");
            Console.WriteLine($"Part 2: {possibles.Last()}");

   
            //DEBUG
            //var machine = new Day19Machine(new long[] { 7224964, 0, 0, 0, 0, 0 });
            //int[] heatMap = new int[instructions.Count];
            //int currentPointer = 0;
            //int instrictionsCount = 0;
            //while (true)
            //{
            //    if (currentPointer < 0 || currentPointer >= instructions.Count)
            //        break;

            //    heatMap[currentPointer] += 1;
            //    var instruction = instructions[currentPointer];
            //    machine.ProcessInstruction(Day16Opcode.seti, new long[] { currentPointer, 0, IP_REGISTER });
            //    machine.ProcessInstruction(instruction.operation, instruction.instruction);
            //    currentPointer = (int)machine.ReadonlyRegister[IP_REGISTER] + 1;
            //    instrictionsCount++;
            //}
            //Console.WriteLine(instrictionsCount);

            //numbersInstructions[number] = instrictionsCount;
            //Console.WriteLine($"{number} = {instrictionsCount}");
            //Console.WriteLine(string.Join(Environment.NewLine, heatMap.Select(x => x.ToString())));

            //if (startNumber % 1000 == 0)
            //    Console.WriteLine($"Start number: {startNumber} = {instrictionsCount}");
            //s.Add((startNumber, instrictionsCount));
            //}


            //Generete value in register 5 (when register 0 set to this value, program halts)
            //Written in C#
            IEnumerable<long> GetNext()
            {
                var current = new long[] { 0, 0, 0, 0, 0, 0 };

                Part1:
                current[4] = current[5] | 65536;
                current[5] = 13284195;
                Part2:
                current[3] = current[4] & 255;
                current[5] = current[5] + current[3];
                current[5] = current[5] & 16777215;
                current[5] = current[5] * 65899;
                current[5] = current[5] & 16777215;
                if (256 > current[4])
                    goto Part4;

                current[3] = 0;
                Part3:
                current[2] = current[3] + 1;
                current[2] = current[2] * 256;
                if (current[2] > current[4])
                {
                    current[4] = current[3];
                    goto Part2;
                }

                current[3] = current[3] + 1;
                goto Part3;


                Part4:
                yield return current[5];
                goto Part1;
            }
        }
    }
}

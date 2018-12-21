using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode
{
    public class Day19
    {
        public static void Run()
        {
            const int IP_REGISTER = 1;

            List<(Day16Opcode operation, long[] instruction)> instructions = new List<(Day16Opcode operation, long[] instruction)>();
            foreach (var line in File.ReadAllLines(@"Day19\Input.txt"))
            {
                var strings = line.Split(' ');
                instructions.Add(((Day16Opcode)Enum.Parse(typeof(Day16Opcode), strings[0]), strings.Skip(1).Select(long.Parse).ToArray()));
            }

            ////part1
            //var machine = new Day18Machine(new long[] { 1, 0, 0, 0, 0, 0 });
            //int currentPointer = 0;
            //while (true)
            //{
            //    if (currentPointer < 0 || currentPointer >= instructions.Count)
            //        break;

            //    var instruction = instructions[currentPointer];
            //    machine.ProcessInstruction(Day16Opcode.seti, new long[] { currentPointer, 0, IP_REGISTER });
            //    machine.ProcessInstruction(instruction.operation, instruction.instruction);
            //    currentPointer = (int)machine.ReadonlyRegister[IP_REGISTER] + 1;
            //}
            //Console.WriteLine(string.Join(",", machine.ReadonlyRegister.Select(x => x.ToString())));

            ////part2 - DEBUG
            //var machine = new Day18Machine(new long[] { 1, 0, 0, 0, 0, 0 });
            //int currentPointer = 0;
            //long counter = 0;
            //while (true)
            //{
            //    if (currentPointer < 0 || currentPointer >= instructions.Count)
            //        break;

            //    var instruction = instructions[currentPointer];
            //    machine.ProcessInstruction(Day16Opcode.seti, new long[] { currentPointer, 0, IP_REGISTER });
            //    if (counter % 10000000 == 0)
            //    {
            //        Console.Write(currentPointer);

            //        Console.WriteLine($"On {counter} instructions: ip={currentPointer} [{string.Join(",", machine.ReadonlyRegister.Select(x => x.ToString()))}]");
            //        Console.ReadLine();
            //    }
            //    machine.ProcessInstruction(instruction.operation, instruction.instruction);
            //    currentPointer = (int)machine.ReadonlyRegister[IP_REGISTER] + 1;

            //    counter++;
            //}
            //Console.WriteLine(string.Join(",", machine.ReadonlyRegister.Select(x => x.ToString())));


            //from DEBUG and instruction set it is "clear" that: 
            //register 5 settels at 10551296
            //instructions 3-7 increase increase register 0 by register 2 if register 2 can divide number 10551296 (= sum of number is register 2)
            //instructions 13-16 halt when register 2 reaches 10551296 (stop summing at this number)
            //therefore:
            // at halt register 0 is sum of all numbers where 10551296 % x == 0 AND x <= 10551296
            // in simple, it calculates sum of all divisor of number at register 5 (which settels at 10551296)
            // here is the same problem solved in C# in optimized way O(n) instead of original O(n2)
            int total = 0;
            for (int i = 1; i <= 10551296; i++)
                if (10551296 % i == 0)
                    total += i;

            Console.WriteLine($"Part 2: {total}");
        }
    }

    public class Day19Machine
    {
        readonly long[] register;

        public long[] ReadonlyRegister => register.ToArray();

        public Day19Machine(long[] registerInitialState)
        {
            register = registerInitialState.ToArray();
        }

        public void ProcessInstruction(Day16Opcode opcode, long[] instruction)
        {
            long A = instruction[0];
            long B = instruction[1];
            long C = instruction[2];

            switch (opcode)
            {
                case Day16Opcode.addr:
                    register[C] = register[A] + register[B];
                    break;
                case Day16Opcode.addi:
                    register[C] = register[A] + B;
                    break;
                case Day16Opcode.mulr:
                    register[C] = register[A] * register[B];
                    break;
                case Day16Opcode.muli:
                    register[C] = register[A] * B;
                    break;
                case Day16Opcode.banr:
                    register[C] = register[A] & register[B];
                    break;
                case Day16Opcode.bani:
                    register[C] = register[A] & B;
                    break;
                case Day16Opcode.borr:
                    register[C] = register[A] | register[B];
                    break;
                case Day16Opcode.bori:
                    register[C] = register[A] | B;
                    break;
                case Day16Opcode.setr:
                    register[C] = register[A];
                    break;
                case Day16Opcode.seti:
                    register[C] = A;
                    break;
                case Day16Opcode.gtir:
                    register[C] = A > register[B] ? 1 : 0;
                    break;
                case Day16Opcode.gtri:
                    register[C] = register[A] > B ? 1 : 0;
                    break;
                case Day16Opcode.gtrr:
                    register[C] = register[A] > register[B] ? 1 : 0;
                    break;
                case Day16Opcode.eqir:
                    register[C] = A == register[B] ? 1 : 0;
                    break;
                case Day16Opcode.eqri:
                    register[C] = register[A] == B ? 1 : 0;
                    break;
                case Day16Opcode.eqrr:
                    register[C] = register[A] == register[B] ? 1 : 0;
                    break;
            }
        }
    }
}

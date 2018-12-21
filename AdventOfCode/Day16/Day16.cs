using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    public class Day16
    {
        private static readonly List<Day16Opcode> allOpcodes = Enum.GetValues(typeof(Day16Opcode)).Cast<Day16Opcode>().ToList();

        public static void Run()
        {
            List<(int opCode, List<Day16Opcode> possibleOpcodes)> learningResutls = new List<(int opCode, List<Day16Opcode> possibleOpcodes)>();
            var lines = System.IO.File.ReadAllLines(@"Day16\Learning.txt");
            for (int i = 0; i < lines.Length;)
            {
                var line = lines[i++];
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var input = new string(line.SkipWhile(x => x != '[').Skip(1).TakeWhile(x => x != ']').ToArray()).Split(',').Select(x => long.Parse(x.Trim())).ToArray();
                line = lines[i++];
                var instruction = line.Split(' ').Select(x => long.Parse(x)).ToArray();
                line = lines[i++];
                var output = new string(line.SkipWhile(x => x != '[').Skip(1).TakeWhile(x => x != ']').ToArray()).Split(',').Select(x => long.Parse(x.Trim())).ToArray();

                learningResutls.Add(GetPossibleOpcodes(input, output, instruction));

            }

            var allRemainingCodes = allOpcodes.ToList();
           
            Dictionary<int, Day16Opcode> numberToOpcodeMapping = new Dictionary<int, Day16Opcode>();

            while (allRemainingCodes.Any())
            {
                for (int opcodeNumber = 0; opcodeNumber < 16; opcodeNumber++)
                {
                    if (numberToOpcodeMapping.ContainsKey(opcodeNumber))
                        continue;

                    var remainnigCodes = allRemainingCodes.ToList();

                    foreach (var learningResult in learningResutls)
                    {
                        if (learningResult.opCode != opcodeNumber)
                            continue;

                        remainnigCodes = remainnigCodes.Intersect(learningResult.possibleOpcodes).ToList();

                        if (remainnigCodes.Count == 1)
                            break;
                    }
                    if (remainnigCodes.Count == 1)
                        numberToOpcodeMapping[opcodeNumber] = remainnigCodes[0];

                }

                allRemainingCodes.RemoveAll(x => numberToOpcodeMapping.ContainsValue(x));
            }

            var machine = new Day16Machine(new long[] { 0, 0, 0, 0 });
            foreach (var item in System.IO.File.ReadAllLines(@"Day16\Input.txt"))
            {
                var data = item.Split(' ').Select(x => long.Parse(x)).ToArray();
                machine.ProcessInstruction(numberToOpcodeMapping[(int)data[0]], data);
            }

            Console.WriteLine(string.Join(",", machine.ReadonlyRegister.Select(x => x.ToString())));


        }

        private static (int opCode, List<Day16Opcode> possibleOpcodes) GetPossibleOpcodes(long[] input, long[] output, long[] instruction)
        {
            List<Day16Opcode> matchingCodes = new List<Day16Opcode>();
            foreach (var item in allOpcodes)
            {
                var machine = new Day16Machine(input);
                machine.ProcessInstruction(item, instruction);
                if (machine.ReadonlyRegister.SequenceEqual(output))
                    matchingCodes.Add(item);
            }

            return ((int)instruction[0], matchingCodes);
        }
    }

    public class Day16Machine
    { 
        readonly long[] register;

        public long[] ReadonlyRegister => register.ToArray();

        public Day16Machine(long[] registerInitialState)
        {
            register = registerInitialState.ToArray();
        }

        public void ProcessInstruction(Day16Opcode opcode, long[] instruction)
        {
            long A = instruction[1];
            long B = instruction[2];
            long C = instruction[3];

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

    public enum Day16Opcode
    {
        addr,
        addi,
        mulr,
        muli,
        banr,
        bani,
        borr,
        bori,
        setr,
        seti,
        gtir,
        gtri,
        gtrr,
        eqir,
        eqri,
        eqrr
    }
}

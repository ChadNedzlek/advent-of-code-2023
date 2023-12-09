using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ChadNedzlek.AdventOfCode.Library;

namespace ChadNedzlek.AdventOfCode.Y2023.CSharp.solvers
{
    public class Problem08 : DualAsyncProblemBase
    {
        protected override async Task ExecutePart1Async(string[] data)
        {
            var inst = data[0].Trim();
            Dictionary<string, (string left, string right)> nodes = new();
            string start = "AAA";
            foreach (var line in data.Skip(2))
            {
                var (n, l, r) = Data.Parse<string, string, string>(line, @"^(...) = \((...), (...)\)$");
                nodes.Add(n, (l, r));
            }

            int i = 0;
            string current = start;
            while (true)
            {
                if (current == "ZZZ")
                {
                    Console.WriteLine($"Took {i} steps to reach {current}");
                    break;
                }

                char c = inst[i % (inst.Length)];
                current = c switch
                {
                    'L' => nodes[current].left,
                    'R' => nodes[current].right,
                };
                i++;
                Helpers.VerboseLine($"Step {i} to {current}");
            }
        }

        protected override async Task ExecutePart2Async(string[] data)
        {
            var inst = data[0].Trim();
            Dictionary<string, (string left, string right)> nodes = new();
            List<string> positions = new List<string>();
            foreach (var line in data.Skip(2))
            {
                var (n, l, r) = Data.Parse<string, string, string>(line, @"^(...) = \((...), (...)\)$");
                nodes.Add(n, (l, r));
                if (n.EndsWith("A"))
                    positions.Add(n);
            }

            LameCheatyMethod(positions, inst, nodes);

            CoolPerfectMethod(positions, inst, nodes);
        }

        private void CoolPerfectMethod(List<string> positions, string inst, Dictionary<string,(string left, string right)> nodes)
        {
            var solves = positions.Select(p => CalculateCycle(inst, nodes, p)).ToList();

            var final = solves.Aggregate(CombineSolutions);
            
            Console.WriteLine($"Solution that isn't cheaty: {final.EndStates.Min() + final.Offset}");
        }

        private PotentialSolution CombineSolutions(PotentialSolution a, PotentialSolution b)
        {
            var newCycle = Helpers.Lcm(a.CycleLength, b.CycleLength);
            var offset = Math.Max(a.Offset, b.Offset);

            HashSet<long> newEnds = new();
            foreach (var e in a.EndStates)
            {
                var newE = (e + a.Offset - offset + a.CycleLength) % a.CycleLength;
                for (long i = 0; i < newCycle; i += a.CycleLength)
                {
                    var x = newE + i;
                    if (b.EndStates.Contains((x - b.Offset) % b.CycleLength + offset))
                    {
                        newEnds.Add(x);
                    }
                }
            }

            var ret = new PotentialSolution($"{a.Name}+{b.Name}", offset, newCycle, newEnds.ToImmutableList());
            Console.WriteLine(ret);
            return ret;
        }

        private static PotentialSolution CalculateCycle(string inst, Dictionary<string, (string left, string right)> nodes, string pos)
        {
            Dictionary<(int index, string location), int> visited = new();
            int i = 0;
            var cur = pos;
            List<int> endPoints = new List<int>();
            while (true)
            {
                if (cur.EndsWith("Z"))
                {
                    endPoints.Add(i);
                }

                int modi = i % inst.Length;

                if (visited.TryGetValue((modi, cur), out var when))
                {
                    var ret = new PotentialSolution(
                        pos,
                        when,
                        i - when,
                        endPoints.Select(e => (long)e - when).ToImmutableList()
                    );
                    Console.WriteLine(ret);
                    return ret;
                }

                visited.Add((modi, cur), i);

                i++;
                cur = inst[modi] switch
                {
                    'L' => nodes[cur].left,
                    'R' => nodes[cur].right,
                };
            }
        }

        public record PotentialSolution(string Name, long Offset, long CycleLength, ImmutableList<long> EndStates)
        {
            public override string ToString()
            {
                return $"{Name} : O={Offset}, C={CycleLength}, E={string.Join(", ", EndStates)}";
            }
        }

        private static void LameCheatyMethod(List<string> positions, string inst, Dictionary<string, (string left, string right)> nodes)
        {
            Dictionary<string, long> times = new();
            foreach (var p in positions)
            {
                int i = 0;
                string current = p;
                while (true)
                {
                    if (current.EndsWith("Z"))
                    {
                        Console.WriteLine($"Took {i} steps to reach {p} => {current}");
                        times[p] = i;
                        break;
                    }

                    char c = inst[i % (inst.Length)];
                    current = c switch
                    {
                        'L' => nodes[current].left,
                        'R' => nodes[current].right,
                    };
                    i++;
                    Helpers.VerboseLine($"Step {i} to {current}");
                }
            }

            var bigGcd = times.Values.Aggregate(1L, (m, x) => Helpers.Lcm(m, x));
            Console.WriteLine($"Try {bigGcd}");
        }
    }
}
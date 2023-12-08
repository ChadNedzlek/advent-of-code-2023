using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ChadNedzlek.AdventOfCode.Library;
using JetBrains.Annotations;
using Spectre.Console;

namespace ChadNedzlek.AdventOfCode.Y2023.CSharp.solvers
{
    public class Problem15 : DualAsyncProblemBase
    {
        protected override async Task ExecutePart1Async(string[] data)
        {
            var parts = string.Join("", data).Split(',');
            long v = 0;
            foreach (var part in parts)
            {
                v += MakeHash(part);
            }
            Console.WriteLine($"Hash is {v}");
        }

        protected override async Task ExecutePart2Async(string[] data)
        {
            var parts = string.Join("", data).Split(',');
            Dictionary<int, List<Lens>> boxes = new Dictionary<int, List<Lens>>();
            foreach (var inst in parts)
            {
                string name;
                char op;
                int value;
                if (inst[^1] == '-')
                {
                    name = inst[..^1];
                    op = '-';
                    value = 0;
                }
                else
                {
                    name = inst[..^2];
                    op = '=';
                    value = inst[^1] - '0';
                }

                var boxNo = MakeHash(name);
                if (!boxes.TryGetValue(boxNo, out var box))
                    boxes.Add(boxNo, box = new List<Lens>());

                switch (op)
                {
                    case '-':
                        box.RemoveAll(l => l.Name == name);
                        break;
                    case '=':
                        var i = box.FindIndex(l => l.Name == name);
                        if (i == -1)
                        {
                            box.Add(new Lens(name, value));
                        }
                        else
                        {
                            box.RemoveAt(i);
                            box.Insert(i, new Lens(name, value));
                        }

                        break;
                }
                
                Helpers.VerboseLine($"After \"{inst}\" (box {boxNo})");
                foreach (var b in boxes.OrderBy(b => b.Key))
                {
                    if (b.Value.Count != 0)
                        Helpers.VerboseLine($"Box {b.Key}: {string.Join(" ", b.Value.Select(l => $"[{l.Name} {l.Value}]"))}");
                }
                Helpers.VerboseLine("");
            }

            long power = 0;
            foreach ((int key, List<Lens> value) in boxes)
            {
                for (var i = 0; i < value.Count; i++)
                {
                    int lensPower = (key + 1) * (i + 1) * value[i].Value;
                    Console.WriteLine($"Lens power for {value[i].Name} is {key + 1} * {i+1} * {value[i].Value} = {lensPower}");
                    power += lensPower;
                }
            }
            Console.WriteLine($"Power = {power}");
        }

        public record Lens(string Name, int Value);

        public static int MakeHash(string s)
        {
            int value = 0;
            foreach (var c in s)
            {
                value += c;
                value *= 17;
                value %= 256;
            }

            return value;
        }
    }
}
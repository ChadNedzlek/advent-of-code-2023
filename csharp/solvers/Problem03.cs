using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ChadNedzlek.AdventOfCode.Library;
using Spectre.Console;

namespace ChadNedzlek.AdventOfCode.Y2023.CSharp.solvers
{
    public class Problem03 : DualAsyncProblemBase
    {
        protected override async Task ExecutePart1Async(string[] data)
        {
            var symbols = new List<Point2I>();
            for (var y = 0; y < data.Length; y++)
            {
                string part = data[y];
                for (var x = 0; x < part.Length; x++)
                {
                    var l = part[x];
                    if (char.IsDigit(l) || l == '.')
                    {
                        continue;
                    }
                    symbols.Add(new (x,y));
                }
            }

            long total = 0;

            void Add(long v)
            {
                if (v == 0)
                    return;
                total += v;
                Console.WriteLine($"Added {v}");
            }

            foreach (var p in symbols)
            {
                HashSet<long> ratios = new();
                var s = data[p.Col][p.Row];
                ratios.Add(GetNumber(data, p.Add(-1, -1)));
                ratios.Add(GetNumber(data, p.Add(0, -1)));
                ratios.Add(GetNumber(data, p.Add(1, -1)));
                ratios.Add(GetNumber(data, p.Add(-1, 0)));
                ratios.Add(GetNumber(data, p.Add(1, 0)));
                ratios.Add(GetNumber(data, p.Add(-1, 1)));
                ratios.Add(GetNumber(data, p.Add(0, 1)));
                ratios.Add(GetNumber(data, p.Add(1, 1)));
                ratios.Remove(0);

                foreach (var value in ratios)
                    Add(value);
            }
            
            Console.WriteLine($"Total {total}");
        }

        private long GetNumber(string[] parts, Point2I p)
        {
            if (!parts.TryGet(p.Col, p.Row, out char c))
                return 0;
            if (c == '.')
                return 0;
            string line = parts[p.Col];
            int s = p.Row;
            int e = p.Row;
            while (s > 0 && char.IsDigit(line[s - 1]))
            {
                s--;
            }
            while (e < line.Length - 1 && char.IsDigit(line[e + 1]))
            {
                e++;
            }

            return long.Parse(line[s..(e + 1)]);
        }

        protected override async Task ExecutePart2Async(string[] data)
        {
            var symbols = new List<Point2I>();
            for (var y = 0; y < data.Length; y++)
            {
                string part = data[y];
                for (var x = 0; x < part.Length; x++)
                {
                    var l = part[x];
                    if (char.IsDigit(l) || l == '.')
                    {
                        continue;
                    }
                    symbols.Add(new (x,y));
                }
            }

            long total = 0;
            foreach (var p in symbols)
            {
                HashSet<long> ratios = new();
                var s = data[p.Col][p.Row];
                ratios.Add(GetNumber(data, p.Add(-1, -1)));
                ratios.Add(GetNumber(data, p.Add(0, -1)));
                ratios.Add(GetNumber(data, p.Add(1, -1)));
                ratios.Add(GetNumber(data, p.Add(-1, 0)));
                ratios.Add(GetNumber(data, p.Add(1, 0)));
                ratios.Add(GetNumber(data, p.Add(-1, 1)));
                ratios.Add(GetNumber(data, p.Add(0, 1)));
                ratios.Add(GetNumber(data, p.Add(1, 1)));
                ratios.Remove(0);

                if (ratios.Count == 2)
                {
                    var mult = ratios.Aggregate(1L, (a, b) => a * b);
                    Console.WriteLine($"Gear ratio: {mult}");
                    total += mult;
                }
            }
            
            Console.WriteLine($"Total {total}");
        }
    }
}
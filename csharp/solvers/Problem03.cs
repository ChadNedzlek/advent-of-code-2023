using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Spectre.Console;

namespace ChadNedzlek.AdventOfCode.Y2023.CSharp.solvers
{
    public class Problem03 : DualAsyncProblemBase
    {
        protected override async Task ExecutePart1Async(IAsyncEnumerable<string> data)
        {
            var parts = await data.ToListAsync();
            var symbols = new List<Point2<int>>();
            for (var y = 0; y < parts.Count; y++)
            {
                string part = parts[y];
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
                var s = parts[p.Y][p.X];
                ratios.Add(GetNumber(parts, p.Add(-1, -1)));
                ratios.Add(GetNumber(parts, p.Add(0, -1)));
                ratios.Add(GetNumber(parts, p.Add(1, -1)));
                ratios.Add(GetNumber(parts, p.Add(-1, 0)));
                ratios.Add(GetNumber(parts, p.Add(1, 0)));
                ratios.Add(GetNumber(parts, p.Add(-1, 1)));
                ratios.Add(GetNumber(parts, p.Add(0, 1)));
                ratios.Add(GetNumber(parts, p.Add(1, 1)));
                ratios.Remove(0);

                foreach (var value in ratios)
                    Add(value);
            }
            
            Console.WriteLine($"Total {total}");
        }

        private long GetNumber(List<string> parts, Point2<int> p)
        {
            if (!parts.TryGet(p.Y, p.X, out char c))
                return 0;
            if (c == '.')
                return 0;
            string line = parts[p.Y];
            int s = p.X;
            int e = p.X;
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

        protected override async Task ExecutePart2Async(IAsyncEnumerable<string> data)
        {
            var parts = await data.ToListAsync();
            var symbols = new List<Point2<int>>();
            for (var y = 0; y < parts.Count; y++)
            {
                string part = parts[y];
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
                var s = parts[p.Y][p.X];
                ratios.Add(GetNumber(parts, p.Add(-1, -1)));
                ratios.Add(GetNumber(parts, p.Add(0, -1)));
                ratios.Add(GetNumber(parts, p.Add(1, -1)));
                ratios.Add(GetNumber(parts, p.Add(-1, 0)));
                ratios.Add(GetNumber(parts, p.Add(1, 0)));
                ratios.Add(GetNumber(parts, p.Add(-1, 1)));
                ratios.Add(GetNumber(parts, p.Add(0, 1)));
                ratios.Add(GetNumber(parts, p.Add(1, 1)));
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
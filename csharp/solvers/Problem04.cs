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
    public class Problem04 : DualAsyncProblemBase
    {
        protected override async Task ExecutePart1Async(string[] data)
        {
            int total = 0;
            foreach (var line in data)
            {
                (string name, string values) = line.Split(':');
                (string wins, string haves) = values.Split('|');
                HashSet<string> win = wins.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToHashSet();
                HashSet<string> have = haves.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToHashSet();
                HashSet<string> overlap = have.Intersect(win).ToHashSet();
                int value = overlap.Count == 0 ? 0 : 1 << (overlap.Count - 1);
                total += value;
                Console.WriteLine($"{name} worth {value}");
            }

            Console.WriteLine($"Points {total}");
        }

        protected override async Task ExecutePart2Async(string[] data)
        {
            long total = 0;
            int i = 0;
            Dictionary<int,long> mult = new() { { 0, 1 } };
            foreach (var line in data)
            {
                long curMult = mult.GetValueOrDefault(i, 1);
                total += curMult;
                (string name, string values) = line.Split(':');
                (string wins, string haves) = values.Split('|');
                HashSet<string> win = wins.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToHashSet();
                HashSet<string> have = haves.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToHashSet();
                HashSet<string> overlap = have.Intersect(win).ToHashSet();
                int value = overlap.Count;
                for (int j = 1; j <= value; j++)
                {
                    mult.AddOrUpdate(i+j, curMult + 1, v => v + curMult);
                }
                Console.WriteLine($"{name} with {curMult} multiples worth {value}");
                i++;
            }

            Console.WriteLine($"Points {total}");
        }
    }
}
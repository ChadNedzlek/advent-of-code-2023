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
    public class Problem06 : DualAsyncProblemBase
    {
        protected override async Task ExecutePart1Async(IAsyncEnumerable<string> data)
        {
            var list = await data.ToListAsync();
            var (_, stime) = list[0].Split(':');
            var (_, sdistanc) = list[1].Split(':');
            var times = stime.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();
            var distances = sdistanc.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();
            long total = 1;
            for (int i = 0; i < times.Count; i++)
            {
                total *= BoringWay(i, times, distances);
            }
            Console.WriteLine($"Multi: {total}");
        }

        private static int BoringWay(int i, List<long> times, List<long> distances)
        {
            Helpers.Verbose($"Race {i} wins with ");
            int perm = 0;
            for (int h = 1; h < times[i]; h++)
            {
                var dist = (times[i] - h) * h;
                if (dist > distances[i])
                {
                    Helpers.Verbose($"{h} ");
                    perm++;
                }
            }

            Helpers.VerboseLine($" for {perm} ways to win");
            return perm;
        }

        private static int MathyWay(int i, long time, long distance)
        {
            var (a, b) = Algorithms.SolveQuadratic(-1, time, -distance);
            var perm = (int)(Math.Floor(b) - Math.Ceiling(a) + 1);

            Helpers.VerboseLine($"Race {i} has {perm} ways to win");
            return perm;
        }

        protected override async Task ExecutePart2Async(IAsyncEnumerable<string> data)
        {
            var list = await data.ToListAsync();
            var (_, stime) = list[0].Split(':');
            var (_, sdistanc) = list[1].Split(':');
            var times = stime.Replace(" ", "").Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();
            var distances = sdistanc.Replace(" ", "").Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();
            long total = 1;
            for (int i = 0; i < times.Count; i++)
            {
                total *= MathyWay(i, times[i], distances[i]);
            }
            Console.WriteLine($"Multi: {total}");
        }
    }
}
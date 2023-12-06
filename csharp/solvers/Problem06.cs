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
            long total = times.Zip(distances, (t,d) => (time:t, distances:d)).Aggregate(1, (mult, cur) => mult * BoringWay(cur.time, cur.distances));
            Console.WriteLine($"Multi: {total}");
        }

        protected override async Task ExecutePart2Async(IAsyncEnumerable<string> data)
        {
            var list = await data.ToListAsync();
            var (_, stime) = list[0].Split(':');
            var (_, sdistanc) = list[1].Split(':');
            var times = stime.Replace(" ", "").Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();
            var distances = sdistanc.Replace(" ", "").Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();
            long total = times.Zip(distances, (t,d) => (time:t, distances:d)).Aggregate(1, (mult, cur) => mult * MathyWay(cur.time, cur.distances));
            Console.WriteLine($"Multi: {total}");
        }

        private static int BoringWay(long time, long distance)
        {
            Helpers.Verbose($"Race wins with ");
            int perm = 0;
            for (int h = 1; h < time; h++)
            {
                var dist = (time - h) * h;
                if (dist > distance)
                {
                    Helpers.Verbose($"{h} ");
                    perm++;
                }
            }

            Helpers.VerboseLine($" for {perm} ways to win");
            return perm;
        }

        private static int MathyWay(long time, long distance)
        {
            // If we spend 'x' time "charging", then we get to go for 'time - x'
            // at speed 'x', which means the distance is
            // (time - x)(x) = distance
            // A little rearranging...
            // time * x - x ^ 2 = distance
            // More math!
            // -x^2 + time*x - distance = 0
            // That's a quadratic equation, we can instantly calculate the overlaps
            var (a, b) = Algorithms.SolveQuadratic(-1, time, -distance);
            
            // They are doubles, because math, so we need to see which discrete points we are
            // bracketing, so floor the top and bottom, and it's inclusive, so add one
            var perm = (int)(Math.Floor(b) - Math.Ceiling(a) + 1);

            Helpers.VerboseLine($"Race has {perm} ways to win");
            return perm;
        }
    }
}
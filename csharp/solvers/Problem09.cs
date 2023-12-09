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
    public class Problem09 : DualAsyncProblemBase
    {
        protected override async Task ExecutePart1Async(string[] data)
        {
            long total = 0;
            foreach (var line in data)
            {
                var n = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();
                long subtotal = n[^1];
                while (true)
                {
                    Helpers.VerboseLine(string.Join(" ", n));
                    n = n.Skip(1).Select((x, i) => x - n[i]).ToList();
                    subtotal += n[^1];
                    if (n.All(i => i == 0))
                    {
                        Helpers.VerboseLine($"Subtotal is {subtotal}");
                        total += subtotal;
                        break;
                    }
                }
            }

            Console.WriteLine($"Total is {total}");
        }

        protected override async Task ExecutePart2Async(string[] data)
        {
            Recusion(data);
            Iteration(data);
        }

        private static void Recusion(string[] data)
        {
            long total = 0;
            foreach (var line in data)
            {
                var n = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();
                long sub = Decend(n);
                Helpers.VerboseLine($"Subtotal : {sub} {string.Join(" ", n)}");
                total += sub;

                long Decend(List<long> d)
                {
                    var m = d.Skip(1).Select((x, i) => x - d[i]).ToList();
                    if (m.All(i => i == 0))
                        return d[0];
                    long sub = Decend(m);
                    Helpers.VerboseLine($".. {sub} {string.Join(" ", d)}");
                    return d[0] - sub;
                }
            }

            Console.WriteLine($"Total is {total}");
        }

        private static void Iteration(string[] data)
        {
            long total = 0;
            foreach (var line in data)
            {
                int mult = 1;
                var n = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();
                long subtotal = n[0];
                while (true)
                {
                    Helpers.VerboseLine(string.Join(" ", n));
                    n = n.Skip(1).Select((x, i) => x - n[i]).ToList();
                    mult *= -1;
                    subtotal += n[0] * mult;
                    if (n.All(i => i == 0))
                    {
                        Helpers.VerboseLine($"Subtotal is {subtotal}");
                        total += subtotal;
                        break;
                    }
                }
            }

            Console.WriteLine($"Total is {total}");
        }
    }
}
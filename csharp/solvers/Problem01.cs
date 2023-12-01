using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Spectre.Console;

namespace ChadNedzlek.AdventOfCode.Y2023.CSharp.solvers
{
    public class Problem01 : AsyncProblemBase
    {
        protected override async Task ExecuteCoreAsync(IAsyncEnumerable<string> data)
        {
            var list = await data.ToListAsync();
            var part1 = list.Select(d => Regex.Replace(d, "[a-z]", ""))
                .Where(d => d.Length != 0)
                .Select(d => d.Length == 2 ? d : $"{d[0]}{d[^1]}").ToList();
            Console.WriteLine($"Lines: {string.Join(", ", part1)}");
            Console.WriteLine($"Sum: {part1.Select(int.Parse).Sum()}");

            IEnumerable<string> replLines = TranslateLines(list)
                .ToList();
            Console.WriteLine($"Repl lines: {string.Join("\n", replLines)}");
            var part2 = replLines
                .Select(d => Regex.Replace(d, "[a-z]", ""))
                .Select(d => $"{d[0]}{d[^1]}").ToList();
            Console.WriteLine($"Spelly lines: {string.Join(", ", part2)}");
            Console.WriteLine($"Spelly Sum: {part2.Select(int.Parse).Sum()}");
        }

        private static IEnumerable<string> TranslateLines(List<string> list)
        {
            foreach (var line in list)
            {
                StringBuilder b = new();
                for (var i = 0; i < line.Length; i++)
                {
                    if (line[i..].StartsWith("one"))
                        b.Append(1);
                    if (line[i..].StartsWith("two"))
                        b.Append(2);
                    if (line[i..].StartsWith("three"))
                        b.Append(3);
                    if (line[i..].StartsWith("four"))
                        b.Append(4);
                    if (line[i..].StartsWith("five"))
                        b.Append(5);
                    if (line[i..].StartsWith("six"))
                        b.Append(6);
                    if (line[i..].StartsWith("seven"))
                        b.Append(7);
                    if (line[i..].StartsWith("eight"))
                        b.Append(8);
                    if (line[i..].StartsWith("nine"))
                        b.Append(9);
                    if (line[i..].StartsWith("zero"))
                        b.Append(0);
                    if (char.IsDigit(line[i]))
                        b.Append(line[i]);
                }

                yield return b.ToString();
            }
        }
    }
}
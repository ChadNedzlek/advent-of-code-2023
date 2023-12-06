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
    public class Problem05 : DualAsyncProblemBase
    {
        protected override async Task ExecutePart1Async(IAsyncEnumerable<string> data)
        {
            string startLabel = null;
            List<long> start = null;
            Dictionary<string, (string target, List<MapRange> ranges)> mappings = new();
            List<MapRange> currentRanges = null;
            await foreach (var line in data)
            {
                if (startLabel == null)
                {
                    (startLabel, string list) = line.Split(':');
                    startLabel = startLabel[..^1];
                    start = list.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();
                    continue;
                }

                if (string.IsNullOrWhiteSpace(line))
                {
                    currentRanges = null;
                    continue;
                }

                if (currentRanges == null)
                {
                    var (from, to) = Data.Parse<string, string>(line, @"([a-z]+)-to-([a-z]+) map:");
                    currentRanges = new List<MapRange>();
                    mappings.Add(from, (to, currentRanges));
                    continue;
                }

                var (dest, src, length) = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
                currentRanges.Add(new MapRange(new (src, length), dest));
            }

            string step = startLabel;
            IEnumerable<long> values = start;
            while (step != "location")
            {
                Helpers.VerboseLine($"{step} : {string.Join(" ", values)}");
                var (next, ranges) = mappings[step];
                values = values.Select(v => Map(v, ranges));
                step = next;
            }

            Helpers.VerboseLine($"{step} : {string.Join(" ", values)}");
            Console.WriteLine($"Lowest: {values.Min()}");
        }

        private long Map(long input, List<MapRange> ranges)
        {
            return ranges.Select(r => r.Map(input)).FirstOrDefault(x => x != null) ?? input;
        }

        protected override async Task ExecutePart2Async(IAsyncEnumerable<string> data)
        {
            string startLabel = null;
            List<RangeL> start = null;
            Dictionary<string, (string target, List<MapRange> ranges)> mappings = new();
            List<MapRange> currentRanges = null;
            await foreach (var line in data)
            {
                if (startLabel == null)
                {
                    (startLabel, string list) = line.Split(':');
                    startLabel = startLabel[..^1];
                    var parts = list.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();
                    start = new List<RangeL>();
                    for (var i = 0; i < parts.Count; i+=2)
                    {
                        start.Add(new RangeL(parts[i], parts[i + 1]));
                    }

                    continue;
                }

                if (string.IsNullOrWhiteSpace(line))
                {
                    currentRanges = null;
                    continue;
                }

                if (currentRanges == null)
                {
                    var (from, to) = Data.Parse<string, string>(line, @"([a-z]+)-to-([a-z]+) map:");
                    currentRanges = new List<MapRange>();
                    mappings.Add(from, (to, currentRanges));
                    continue;
                }

                var (dest, src, length) = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
                currentRanges.Add(new MapRange(new (src, length), dest));
            }

            string step = startLabel;
            IEnumerable<RangeL> values = start;
            while (step != "location")
            {
                Console.WriteLine($"{step} : {string.Join(" ", values)}");
                var (next, ranges) = mappings[step];
                var unresolved = values;
                var resolved = new List<RangeL>();
                foreach (var r in ranges)
                {
                    (unresolved, List<RangeL> mapped) = r.Map(unresolved);
                    resolved.AddRange(mapped);
                }

                values = unresolved.Concat(resolved);
                step = next;
            }

            Console.WriteLine($"{step} : {string.Join(" ", values)}");
            Console.WriteLine($"Lowest: {values.Min(v => v.Start)}");
        }

        private record struct MapRange(RangeL Source, long DestinationStart)
        {
            public override string ToString() => $"{Source}=>{DestinationStart}-{DestinationStart + Source.Length}";

            public long? Map(long input)
            {
                if (input < Source.Start)
                    return null;
                if (input >= Source.End)
                    return null;
                return DestinationStart + (input - Source.Start);
            }

            public (List<RangeL> unmapped, List<RangeL> mapped) Map(IEnumerable<RangeL> input)
            {
                List<RangeL> mapped = new List<RangeL>();
                List<RangeL> unmapped = new List<RangeL>();
                foreach (var i in input)
                {
                    i.SpliceOut(Source, out var before, out var mid, out var after);
                    if (before.HasValue)
                        unmapped.Add(before.Value);
                    if (after.HasValue)
                        unmapped.Add(after.Value);
                    if (mid.HasValue)
                    {
                        mapped.Add(mid.Value with { Start = Map(mid.Value.Start).Value });
                    }
                }

                return (unmapped, mapped);
            }
        }
    }
}
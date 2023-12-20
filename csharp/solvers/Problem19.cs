using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ChadNedzlek.AdventOfCode.Library;
using Spectre.Console;

namespace ChadNedzlek.AdventOfCode.Y2023.CSharp.solvers
{
    public class Problem19 : DualAsyncProblemBase
    {
        protected override async Task ExecutePart1Async(string[] data)
        {
            var e = data.ToList().GetEnumerator();
            Dictionary<string, Workflow> workflows = new();
            while (e.MoveNext() && !string.IsNullOrWhiteSpace(e.Current))
            {
                (string name, string rest) = e.Current.Split('{');
                string[] instructions = rest[..^1].Split(',');
                string final = instructions[^1];
                bool accept = final == "A";
                Target target = ParseTarget(final);
                var rules = instructions[..^1]
                    .Select(r => r.Split(':'))
                    .Select(p => new Rule(p[0][0], p[0][1], long.Parse(p[0][2..]), ParseTarget(p[1])))
                    .ToImmutableList();
                
                workflows.Add(name, new Workflow(name, rules, target));
            }

            List<Dictionary<char, long>> accepted = new();
            List<Dictionary<char, long>> rejected = new();
            while (e.MoveNext())
            {
                string workflow = "in";
                Dictionary<char, long> values = e.Current[1..^1]
                    .Split(",")
                    .Select(p => p.Split('='))
                    .ToDictionary(p => p[0][0], p => long.Parse(p[1]));
                while (true)
                {
                    var w = workflows[workflow];
                    var t = EvaluateRules(w, values);

                    if (t.Workflow == null)
                    {
                        if (t.Accept)
                            accepted.Add(values);
                        else
                            rejected.Add(values);
                        break;
                    }

                    workflow = t.Workflow;
                }
            }
            
            Console.WriteLine($"Single rule sums {accepted.Sum(a => a.Values.Sum())}");
            
            ImmutableDictionary<char, RangeL> input = ImmutableDictionary<char, RangeL>.Empty;
            input = input.Add('x', new RangeL(1, 4000));
            input = input.Add('m', new RangeL(1, 4000));
            input = input.Add('a', new RangeL(1, 4000));
            input = input.Add('s', new RangeL(1, 4000));
            Queue<(string workflows, ImmutableDictionary<char, RangeL> values)> q = new();
            q.Enqueue(("in", input));
            List<ImmutableDictionary<char, RangeL>> acceptedRanges = new();
            List<ImmutableDictionary<char, RangeL>> rejectedRanges = new();
            while (q.TryDequeue(out var item))
            {
                (string w, ImmutableDictionary<char, RangeL> v) = item;

                foreach ((Target target, ImmutableDictionary<char, RangeL> values) in EvaluateAllRules(workflows[w], v))
                {
                    if (target.Workflow == null)
                    {
                        if (target.Accept)
                        {
                            acceptedRanges.Add(values);
                        }
                        else
                        {
                            rejectedRanges.Add(values);
                        }

                        continue;
                    }

                    q.Enqueue((target.Workflow, values));
                }
            }
            
            Console.WriteLine($"Full combos = {acceptedRanges.Sum(CountCombos)}");
        }

        private static long CountCombos(ImmutableDictionary<char, RangeL> v)
        {
            return v.Values.Aggregate(1L, (a,b) => a * b.Length);
        }

        private static IEnumerable<(Target target, ImmutableDictionary<char, RangeL> values)> EvaluateAllRules(Workflow w, ImmutableDictionary<char, RangeL> values)
        {
            foreach (var rule in w.Rules)
            {
                RangeL? pass = default;
                RangeL? fail = default;
                RangeL currentMeasure = values[rule.Measure];
                switch (rule.Operation)
                {
                    case '<':
                        currentMeasure.Splice(new RangeL(1, rule.Threshold - 1), out RangeL? _, out pass, out fail);
                        break;
                    case '>':
                        currentMeasure.Splice(new RangeL(rule.Threshold + 1, 4000), out fail, out pass, out RangeL? _);
                        break;
                }

                if (pass.HasValue)
                {
                    yield return (rule.Target, values.SetItem(rule.Measure, pass.Value));
                }

                if (!fail.HasValue)
                    yield break;
                
                values = values.SetItem(rule.Measure, fail.Value);
            }

            if (values.Values.All(v => v.Length != 0))
            {
                yield return (w.Target, values);
            }
        }

        private static Target EvaluateRules(Workflow w, Dictionary<char, long> values)
        {
            foreach (var rule in w.Rules)
            {
                var v = values[rule.Measure];
                var t = rule.Threshold;
                bool match = rule.Operation switch
                {
                    '<' => v < t,
                    '>' => v > t,
                };

                if (match)
                {
                    return rule.Target;
                }
            }

            return w.Target;
        }

        private Target ParseTarget(string final)
        {
            return final switch
            {
                "A" => new Target(null, true),
                "R" => new Target(null, false),
                var t => new Target(t, true),
            };
        }

        public class Workflow
        {
            public string Name { get; }
            public ImmutableList<Rule> Rules { get; }
            public Target Target { get; }

            public Workflow(string name, ImmutableList<Rule> rules, Target target)
            {
                Name = name;
                Rules = rules;
                Target = target;
            }
        }

        public record struct Rule(char Measure, char Operation, long Threshold, Target Target);

        public record struct Target(string Workflow, bool Accept);

        protected override async Task ExecutePart2Async(string[] data)
        {
            await base.ExecutePart2Async(data);
        }
    }
}
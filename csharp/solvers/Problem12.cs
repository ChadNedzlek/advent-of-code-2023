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
    public class Problem12 : DualAsyncProblemBase
    {
        private static Dictionary<State, long> _solvedCache = new Dictionary<State, long>
        {
            // Freebie: if there is nothing int the state, there is 1 solution
            { State.GetState("", ImmutableList<int>.Empty), 1 } };

        protected override async Task ExecutePart1Async(string[] data)
        {
            long total = 0;
            foreach (var line in data)
            {
                if (line.StartsWith("//"))
                    continue;
                var (pattern, nums) = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var num = nums.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToImmutableList();
                long sub = CountValidSubstitutions(pattern, num);
                total += sub;
            }
            Console.WriteLine($"Total matches {total}");
        }

        protected override async Task ExecutePart2Async(string[] data)
        {
            await ExecutePart1Async(data);
            
            long unfoldedTotal = 0;
            foreach (var line in data)
            {
                if (line.StartsWith("//"))
                    continue;
                var (pattern, nums) = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var unfoldedPattern = string.Join("?", Enumerable.Repeat(pattern, 5));
                var unfoldedNums = string.Join(",", Enumerable.Repeat(nums, 5));
                var unfoldedNum = unfoldedNums.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToImmutableList();
                long unsub = CountValidSubstitutions(unfoldedPattern, unfoldedNum);
                unfoldedTotal += unsub;
            }
            Console.WriteLine($"Unfolded matches {unfoldedTotal}");
        }

        protected override Task ExecuteTests()
        {
            foreach (var dist in Algorithms.Distribute(6, 4))
            {
                Console.WriteLine(string.Join(" ", dist));
            }
            Environment.Exit(0);
            return Task.CompletedTask;
        }

        private static long CountValidSubstitutions(string pattern, ImmutableList<int> num)
        {
            Stack<State> unsolved = new Stack<State>();
            var initState = State.GetState(pattern, num);
            unsolved.Push(initState);
            while (unsolved.TryPop(out var s))
            {
                if (_solvedCache.ContainsKey(s))
                {
                    continue;
                }

                if (s.Signature == "#. 2")
                {
                }

                if (s.Pattern == "")
                {
                    _solvedCache.Add(s, s.Counts.Count == 0 ? 1 : 0);
                    continue;
                }

                if (s.Pattern.Length < s.Counts.Sum() + s.Counts.Count - 1)
                {
                    _solvedCache.Add(s, 0);
                    continue;
                }

                switch (s.Pattern[0])
                {
                    case '.':
                    {
                        var right = State.GetState(s.Pattern[1..], s.Counts);
                        if (_solvedCache.TryGetValue(right, out var r))
                        {
                            _solvedCache.Add(s, r);
                            continue;
                        }

                        unsolved.Push(s);
                        unsolved.Push(right);
                        continue;
                    }

                    case '#':
                    {
                        if (s.Counts.Count == 0)
                        {
                            _solvedCache.Add(s, 0);
                            continue;
                        }

                        if (s.Counts[0] == 1)
                        {
                            if (s.Pattern == "#")
                            {
                                _solvedCache.Add(s, 1);
                                continue;
                            }

                            if (s.Pattern[1] == '#')
                            {
                                // It starts with "##", but our first number is 1
                                _solvedCache.Add(s, 0);
                                continue;
                            }

                            var right = State.GetState(s.Pattern[2..], s.Counts.RemoveAt(0));
                            if (_solvedCache.TryGetValue(right, out var r))
                            {
                                _solvedCache.Add(s, r);
                                continue;
                            }
                            unsolved.Push(s);
                            unsolved.Push(right);
                            continue;
                        }
                        else
                        {
                            if (s.Pattern[1] == '.')
                            {
                                _solvedCache.Add(s, 0);
                                continue;
                            }

                            var right = State.GetState('#' + s.Pattern[2..], s.Counts.SetItem(0, s.Counts[0]-1));
                            if (_solvedCache.TryGetValue(right, out var r))
                            {
                                _solvedCache.Add(s, r);
                                continue;
                            }

                            unsolved.Push(s);
                            unsolved.Push(right);
                            continue;
                        }
                    }

                    case '?':
                    {
                        var left = State.GetState('.' + s.Pattern[1..], s.Counts);
                        var right = State.GetState('#' + s.Pattern[1..], s.Counts);

                        if (_solvedCache.TryGetValue(left, out var l) &&
                            _solvedCache.TryGetValue(right, out var r))
                        {
                            _solvedCache.Add(s, l + r);
                            continue;
                        }
                        
                        unsolved.Push(s);
                        unsolved.Push(left);
                        unsolved.Push(right);
                        continue;
                    }
                }
            }

            Helpers.VerboseLine($"Found {_solvedCache[initState]} matches in {initState.Signature}");
            return _solvedCache[initState];
        }

        public readonly struct State : IEquatable<State>
        {
            public string Pattern { get; }
            public ImmutableList<int> Counts { get; }
            public string Signature { get; }

            private State(string pattern, ImmutableList<int> counts, string signature)
            {
                Pattern = pattern;
                Counts = counts;
                Signature = signature;
            }

            public static State GetState(string pattern, ImmutableList<int> counts)
            {
                var sig = CalculateSignature(pattern, counts);
                return new State(pattern, counts, sig);
            }
            
            public bool Equals(State other)
            {
                return Signature == other.Signature;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (obj.GetType() != GetType()) return false;
                return Equals((State)obj);
            }

            public override int GetHashCode()
            {
                return Signature.GetHashCode();
            }

            public override string ToString() => Signature;
        }

        public static string CalculateSignature(string pattern, ImmutableList<int> counts)
        {
            return pattern + ' ' + string.Join(",", counts);
        }
    }
}
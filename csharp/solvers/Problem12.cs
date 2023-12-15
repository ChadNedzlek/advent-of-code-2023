using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
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
        private static Dictionary<BasicState, long> _solvedCache = new Dictionary<BasicState, long>
        {
            // Freebie: if there is nothing int the state, there is 1 solution
            { new BasicState("", ImmutableList<int>.Empty), 1 } };

        private static readonly TaskBasedMemoSolver<TaskSolveState, long> _taskSolver = new();
        private static readonly CallbackMemoSolver<CallbackSolveState, long> _callbackSolver = new();

        protected override async Task ExecutePart1Async(string[] data)
        {
            long total = 0;
            foreach (var line in data)
            {
                if (line.StartsWith("//"))
                    continue;
                var (pattern, nums) = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var num = nums.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToImmutableList();
                long sub = SolveTaskBasked(pattern, num);
                total += sub;
            }
            Console.WriteLine($"Total matches {total}");
        }

        protected override async Task ExecutePart2Async(string[] data)
        {
            Stopwatch s = Stopwatch.StartNew();
            await ExecuteAllWith(data, SolveCallbackBased);
            Console.WriteLine($"=== Callback based time {s.Elapsed} ===");
            s.Restart();
            await ExecuteAllWith(data, SolveInline);
            Console.WriteLine($"=== Inline time {s.Elapsed} ===");
            s.Restart();
            await ExecuteAllWith(data, SolveTaskBasked);
            Console.WriteLine($"=== Task based time {s.Elapsed} ===");
            s.Restart();
        }

        private async Task<long> ExecuteAllWith(string[] data, Func<string, ImmutableList<int>, long> solver)
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
                long unsub = solver(unfoldedPattern, unfoldedNum);
                unfoldedTotal += unsub;
            }
            Console.WriteLine($"Unfolded matches {unfoldedTotal}");
            return unfoldedTotal;
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

        private class TaskSolveState : ITaskMemoState<TaskSolveState, long>, IEquatable<TaskSolveState>
        {
            public TaskSolveState(string pattern, ImmutableList<int> counts)
            {
                Pattern = pattern;
                Counts = counts;
                Signature = CalculateSignature(pattern, counts);
            }

            public string Pattern { get; }
            public ImmutableList<int> Counts { get; }
            public string Signature { get; }
            
            public async Task<long> Solve(IAsyncSolver<TaskSolveState, long> solver)
            {
                if (Pattern == "")
                    return Counts.Count == 0 ? 1 : 0;

                if (Pattern.Length < Counts.Sum() + Counts.Count - 1)
                    return 0;
                
                switch (Pattern[0])
                {
                    case '.':
                    {
                        var right = new TaskSolveState(Pattern[1..], Counts);
                        return await solver.GetSolutionAsync(right);
                    }

                    case '#':
                    {
                        if (Counts.Count == 0)
                        {
                            return 0;
                        }

                        if (Counts[0] == 1)
                        {
                            if (Pattern == "#")
                            {
                                return 1;
                            }

                            if (Pattern[1] == '#')
                            {
                                // It starts with "##", but our first number is 1
                                return 0;
                            }

                            var right = new TaskSolveState(Pattern[2..], Counts.RemoveAt(0));
                            return await solver.GetSolutionAsync(right);
                        }
                        else
                        {
                            if (Pattern[1] == '.')
                            {
                                return 0;
                            }

                            var right = new TaskSolveState('#' + Pattern[2..], Counts.SetItem(0, Counts[0]-1));
                            return await solver.GetSolutionAsync(right);
                        }
                    }

                    case '?':
                    {
                        var left = new TaskSolveState('.' + Pattern[1..], Counts);
                        var right = new TaskSolveState('#' + Pattern[1..], Counts);

                        return await solver.GetSolutionAsync(left) + await solver.GetSolutionAsync(right);
                    }
                    
                    default:
                        throw new InvalidOperationException();
                }
            }

            public bool Equals(TaskSolveState other)
            {
                if (ReferenceEquals(other, null)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Signature == other.Signature;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((TaskSolveState)obj);
            }

            public override int GetHashCode()
            {
                return Signature.GetHashCode();
            }
        }
        
        public static long SolveTaskBasked(string pattern, ImmutableList<int> num)
        {
            var result = _taskSolver.Solve(new TaskSolveState(pattern, num));
            
            Helpers.VerboseLine($"Found {result} matches in {pattern}");

            return result;
        }
        
        private class CallbackSolveState : CallbackSolvable<CallbackSolveState, long>, IEquatable<CallbackSolveState>
        {
            public string Pattern { get; }
            public ImmutableList<int> Counts { get; }
            public string Signature { get; }

            public CallbackSolveState(string pattern, ImmutableList<int> counts)
            {
                Pattern = pattern;
                Counts = counts;
                Signature = CalculateSignature(pattern, counts);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (obj.GetType() != GetType()) return false;
                return Equals((CallbackSolveState)obj);
            }

            public override int GetHashCode()
            {
                return Signature.GetHashCode();
            }
            
            public bool Equals(CallbackSolveState other)
            {
                return Signature == other.Signature;
            }

            public override ISolution<CallbackSolveState, long> Solve()
            {
                if (Pattern == "")
                    return Immediate(Counts.Count == 0 ? 1L : 0L);

                if (Pattern.Length < Counts.Sum() + Counts.Count - 1)
                    return this.Immediate(0L);
                
                switch (Pattern[0])
                {
                    case '.':
                    {
                        var right = new CallbackSolveState(Pattern[1..], Counts);
                        return Delegate(right);
                    }

                    case '#':
                    {
                        if (Counts.Count == 0)
                        {
                            return Immediate(0);
                        }

                        if (Counts[0] == 1)
                        {
                            if (Pattern == "#")
                            {
                                return Immediate(1);
                            }

                            if (Pattern[1] == '#')
                            {
                                // It starts with "##", but our first number is 1
                                return Immediate(0);
                            }

                            var right = new CallbackSolveState(Pattern[2..], Counts.RemoveAt(0));
                            return Delegate(right);
                        }
                        else
                        {
                            if (Pattern[1] == '.')
                            {
                                return Immediate(0);
                            }

                            var right = new CallbackSolveState('#' + Pattern[2..], Counts.SetItem(0, Counts[0]-1));
                            return Delegate(right);
                        }
                    }

                    case '?':
                    {
                        var left = new CallbackSolveState('.' + Pattern[1..], Counts);
                        var right = new CallbackSolveState('#' + Pattern[1..], Counts);
                        return Delegate(left, right, (l, r) => l + r);
                    }
                    
                    default:
                        throw new InvalidOperationException();
                }
            }
        }
        
        public static long SolveCallbackBased(string pattern, ImmutableList<int> num)
        {
            var result = _callbackSolver.Solve(new CallbackSolveState(pattern, num));
            
            Helpers.VerboseLine($"Found {result} matches in {pattern}");

            return result;
        }

        private static long SolveInline(string pattern, ImmutableList<int> num)
        {
            Stack<BasicState> unsolved = new Stack<BasicState>();
            var initState = new BasicState(pattern, num);
            unsolved.Push(initState);
            while (unsolved.TryPop(out var s))
            {
                if (_solvedCache.ContainsKey(s))
                {
                    continue;
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
                        var right = new BasicState(s.Pattern[1..], s.Counts);
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

                            var right = new BasicState(s.Pattern[2..], s.Counts.RemoveAt(0));
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

                            var right = new BasicState('#' + s.Pattern[2..], s.Counts.SetItem(0, s.Counts[0]-1));
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
                        var left = new BasicState('.' + s.Pattern[1..], s.Counts);
                        var right = new BasicState('#' + s.Pattern[1..], s.Counts);

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

        public class BasicState
        {
            public string Pattern { get; }
            public ImmutableList<int> Counts { get; }
            public string Signature { get; }

            public BasicState(string pattern, ImmutableList<int> counts)
            {
                Pattern = pattern;
                Counts = counts;
                Signature = CalculateSignature(pattern, counts);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (obj.GetType() != GetType()) return false;
                return Equals((BasicState)obj);
            }

            public override int GetHashCode()
            {
                return Signature.GetHashCode();
            }
            
            public bool Equals(BasicState other)
            {
                return Signature == other.Signature;
            }
        }

        public static string CalculateSignature(string pattern, ImmutableList<int> counts)
        {
            return pattern + ' ' + string.Join(",", counts);
        }
    }
}
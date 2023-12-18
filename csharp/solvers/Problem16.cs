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
    public class Problem16 : DualAsyncProblemBase
    {
        protected override async Task ExecutePart1Async(string[] data)
        {
            TaskBasedMemoSolver<AsyncProblem, bool[,]> solver = new TaskBasedMemoSolver<AsyncProblem, bool[,]>(true);
            var result = solver.Solve(new AsyncProblem(data.ToCharArray(), new bool[data.Length, data[0].Length],
                new GPoint2I(0, -1), new GPoint2I(0, 1)));

            Console.WriteLine($"Total energized = {result.Cast<bool>().Count(r => r)}");
        }

        protected override async Task ExecutePart2Async(string[] data)
        {
            char[,] map = data.ToCharArray();
            
            var testEn = new TaskBasedMemoSolver<AsyncProblem, bool[,]>(true)
                .Solve(new AsyncProblem(map,
                    new bool[data.Length, data[0].Length],
                    new GPoint2I(-1, 3),
                    new GPoint2I(1, 0)));
            var test = testEn
                .Cast<bool>()
                .Count(i => i);
            
            for (int r = 0; r < data.Length; r++)
            {
                for (int c = 0; c < data[0].Length; c++)
                {
                    Helpers.Verbose(testEn[r, c] ? "#" : ".");
                }
            
                Helpers.VerboseLine("");
            }
            
            int best = 0;
            for (int r = 0; r < data.Length; r++)
            {
                var right = new TaskBasedMemoSolver<AsyncProblem, bool[,]>(true)
                    .Solve(new AsyncProblem(map,
                        new bool[data.Length, data[0].Length],
                        new GPoint2I(r, -1),
                        new GPoint2I(0, 1)))
                    .Cast<bool>()
                    .Count(i => i);
                
                if (right > best)
                    best = right;

                var left = new TaskBasedMemoSolver<AsyncProblem, bool[,]>(true)
                    .Solve(new AsyncProblem(map,
                        new bool[data.Length, data[0].Length],
                        new GPoint2I(r, data[r].Length),
                        new GPoint2I(0, -1)))
                    .Cast<bool>()
                    .Count(i => i);
                if (left > best)
                    best = left;
            }

            for (int c = 0; c < data[0].Length; c++)
            {
                var down = new TaskBasedMemoSolver<AsyncProblem, bool[,]>(true)
                    .Solve(new AsyncProblem(
                        map,
                        new bool[data.Length, data[0].Length],
                        (-1, c),
                        (1, 0)))
                    .Cast<bool>()
                    .Count(i => i);
                if (down > best)
                    best = down;
                var up = new TaskBasedMemoSolver<AsyncProblem, bool[,]>(true)
                    .Solve(new AsyncProblem(map,
                        new bool[data.Length, data[0].Length],
                        new GPoint2I(data.Length, c),
                        new GPoint2I(-1, 0)))
                    .Cast<bool>()
                    .Count(i => i);
                if (up > best)
                    best = up;

            }

            Console.WriteLine($"Total energized = {best}");
        }

        private class AsyncProblem : TaskBasedMemoSolver<AsyncProblem, bool[,]>, ITaskMemoState<AsyncProblem, bool[,]>, IEquatable<AsyncProblem>
        {
            private readonly char[,] _map;
            private readonly bool[,] _energized;

            public AsyncProblem(
                char[,] map,
                bool[,] energized,
                GPoint2I source,
                GPoint2I direction)
            {
                _map = map;
                _energized = energized;
                Source = source;
                Direction = direction;
            }

            private GPoint2I Direction { get; }
            private GPoint2I Source { get; }

            private AsyncProblem Continue(GPoint2I source, GPoint2I dir) => new AsyncProblem(_map, _energized, source, dir);

            public async Task<bool[,]> Solve(IAsyncSolver<AsyncProblem, bool[,]> solver)
            {
                _energized.TrySet(Source.Row, Source.Col, true);
                var cur = Source.Add(Direction);
                _energized.TrySet(cur.Row, cur.Col, true);
                char value;
                while (_map.TryGet(cur.Row, cur.Col, out value) && value == '.')
                {
                    _energized[cur.Row, cur.Col] = true;
                    cur = cur.Add(Direction);
                }

                if (_map.TryGet(cur.Row, cur.Col, out value))
                {
                    _energized[cur.Row, cur.Col] = true;
                    return (value, Direction.Row, Direction.Col) switch
                    {
                        // Bounce
                        ('|', _, 0) or ('-', 0, _) => await solver.GetSolutionAsync(Continue(cur, Direction)) ?? _energized,
                        ('-', _, 0) => await solver.GetSolutionAsync(new AsyncProblem(
                            _map,
                            await solver.GetSolutionAsync(Continue(cur, new GPoint2I(0, -1))) ?? _energized,
                            cur,
                            new GPoint2I(0, 1)
                            )) ?? _energized,
                        ('|', 0, _) => await solver.GetSolutionAsync(new AsyncProblem(
                            _map,
                            await solver.GetSolutionAsync(Continue(cur, new GPoint2I(-1, 0))) ?? _energized,
                            cur,
                            new GPoint2I(1, 0) 
                        ))?? _energized,
                        ('/', var r, var c) => await solver.GetSolutionAsync(Continue(cur, new GPoint2I(-c, -r))) ?? _energized,
                        ('\\', var r, var c) => await solver.GetSolutionAsync(Continue(cur, new GPoint2I(c, r))) ?? _energized,
                    };
                }

                return _energized;
            }


            public bool Equals(AsyncProblem other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Direction.Equals(other.Direction) && Source.Equals(other.Source);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((AsyncProblem)obj);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Direction, Source);
            }
        }
    }
}
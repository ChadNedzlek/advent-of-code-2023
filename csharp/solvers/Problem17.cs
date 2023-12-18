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
    public class Problem17 : DualAsyncProblemBase
    {
        protected override async Task ExecutePart1Async(string[] data)
        {
            var map = data.ToCharArray();
            int nRows = map.GetLength(0);
            int nCols = map.GetLength(1);
            var result = Algorithms.PrioritySearch(
                new State(map, 0, 3, (0, 0), (0, 0), 0, 0),
                s => s.GetNext(),
                s => s.Location == (nRows - 1, nCols - 1),
                s => -s.Location.Row + -s.Location.Col + s.Cost,
                s => s.GetIdentity(),
                s => s.Cost,
                (a,b) => a > b
            );
            Console.WriteLine($"Total cost = {result.Cost}");
        }

        public record State(
            char[,] Map,
            int MinConsecutive,
            int MaxConsecutive,
            GPoint2I Location,
            GPoint2I Direction,
            int Consecutive,
            long Cost)
        {
            public IEnumerable<State> GetNext()
            {
                if (Direction.Equals(new GPoint2I(0, 0)))
                {
                    yield return this with
                    {
                        Direction = (0, 1), Location = (0, 1), Consecutive = 1, Cost = Map[0, 1] - '0'
                    };
                    yield return this with
                    {
                        Direction = (1, 0), Location = (1, 0), Consecutive = 1, Cost = Map[1, 0] - '0'
                    };
                    yield break;
                }

                char cost;
                if (Consecutive < MaxConsecutive)
                {
                    var n = Location.Add(Direction);
                    if (Map.TryGet(n, out cost))
                    {
                        yield return this with
                        {
                            Location = n, Consecutive = Consecutive + 1, Cost = Cost + (cost - '0')
                        };
                    }
                }

                if (Consecutive >= MinConsecutive)
                {
                    {
                        var lDir = new GPoint2I(Direction.Col, Direction.Row);
                        var lLoc = Location.Add(lDir);
                        if (Map.TryGet(lLoc, out cost))
                        {
                            yield return this with
                            {
                                Location = lLoc, Direction = lDir, Consecutive = 1, Cost = Cost + (cost - '0')
                            };
                        }
                    }

                    {
                        var rDir = new GPoint2I(-Direction.Col, -Direction.Row);
                        var rLoc = Location.Add(rDir);
                        if (Map.TryGet(rLoc, out cost))
                        {
                            yield return this with
                            {
                                Location = rLoc, Direction = rDir, Consecutive = 1, Cost = Cost + (cost - '0')
                            };
                        }
                    }
                }
            }

            public (GPoint2I Location, GPoint2I Direction, int Consecutive) GetIdentity()
            {
                return (Location, Direction, Consecutive);
            }
        }

        protected override async Task ExecutePart2Async(string[] data)
        {
            var map = data.ToCharArray();
            int nRows = map.GetLength(0);
            int nCols = map.GetLength(1);
            var result = Algorithms.PrioritySearch(
                new State(map, 4, 10, (0, 0),  (0, 0), 0, 0),
                s => s.GetNext(),
                s => s.Location == (nRows - 1, nCols - 1),
                s => -s.Location.Row + -s.Location.Col + s.Cost,
                s => s.GetIdentity(),
                s => s.Cost,
                (a,b) => a > b
            );
            Console.WriteLine($"Total cost = {result.Cost}");
        }
    }
}
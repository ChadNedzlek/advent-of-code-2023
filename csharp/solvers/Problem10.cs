using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ChadNedzlek.AdventOfCode.Library;
using Spectre.Console;

namespace ChadNedzlek.AdventOfCode.Y2023.CSharp.solvers
{
    public class Problem10 : DualAsyncProblemBase
    {
        protected override async Task ExecutePart1Async(string[] data)
        {
            int cRows = data.Length;
            int cCols = data[0].Length;
            long [,] dist = new long[cRows, cCols];

            Point2I s = new();
            char sl = '\0';
            for (int r = 0; r < cRows; r++)
            {
                for (int c = 0; c < cCols; c++)
                {
                    if (data[r][c] == 'S')
                    {
                        s = new(r, c);
                        if (data.TryGet(r - 1, c, out var l) && "|F7".Contains(l))
                        {
                            if (data.TryGet(r, c - 1, out l) && "-FL".Contains(l))
                            {
                                sl = 'J';
                            }
                            else if (data.TryGet(r, c + 1, out l) && "-J7".Contains(l))
                            {
                                sl = 'L';
                            }
                            else
                            {
                                sl = '|';
                            }
                        }
                        else if (data.TryGet(r + 1, c, out l) && "|LJ".Contains(l))
                        {
                            if (data.TryGet(r, c - 1, out l) && "-FL".Contains(l))
                            {
                                sl = '7';
                            }
                            else
                            {
                                sl = 'F';
                            }
                        }
                        else
                        {
                            sl = '-';
                        }
                    }
                }
            }

            MarkNeighbors(s.X, s.Y, 0);

            bool runLoop = true;
            for (int d = 1; runLoop; d++)
            {
                runLoop = false;
                for (int r = 0; r < cRows; r++)
                {
                    for (int c = 0; c < cCols; c++)
                    {
                        if (dist[r,c] == d)
                        {
                            runLoop |= MarkNeighbors(r, c, d);
                        }
                    }
                }
            }

            long highest = dist.Cast<long>().Max();

            long insideCount = 0;
            for (int r = 0; r < cRows; r++)
            {
                bool inside = false;
                char bend = '-';
                for (int c = 0; c < cCols; c++)
                {
                    if (dist[r, c] == 0)
                    {
                        if (inside)
                        {
                            insideCount++;
                            dist[r, c] = -1;
                        }
                    }
                    else
                    {
                        char cur = GetMap(r, c);
                        if (cur == '|')
                            inside = !inside;
                        else if (cur == 'J')
                        {
                            if (bend == 'F')
                                inside = !inside;
                            bend = '-';
                        }
                        else if (cur == '7')
                        {
                            if (bend == 'L')
                                inside = !inside;
                            bend = '-';
                        }
                        else if ("FL".Contains(cur))
                            bend = cur;
                    }
                }
            }

            if (Helpers.IncludeVerboseOutput)
            {
                for (int r = 0; r < cRows; r++)
                {
                    for (int c = 0; c < cCols; c++)
                    {
                        if (dist[r,c] == 0)
                            Helpers.Verbose(". ");
                        else if (dist[r,c] == -1)
                            Helpers.Verbose("* ");
                        else
                            Helpers.Verbose($"{dist[r,c]%10} ");
                    }
                    Helpers.VerboseLine("");
                }
            }
            
            Console.WriteLine($"Furthest point is {highest}");
            Console.WriteLine($"Enclosed points {insideCount}");

            bool TrySetMin(int r, int c, long d)
            {
                if (dist[r, c] != 0 && dist[r, c] < d)
                    return false;

                dist[r, c] = d;
                return true;
            }

            bool MarkNeighbors(int r, int c, int d)
            {
                var l = GetMap(r, c);
                return l switch
                {
                    '.' => false,
                    '|' => TrySetMin(r - 1, c, d + 1) | TrySetMin(r + 1, c, d + 1),
                    '-' => TrySetMin(r, c - 1, d + 1) | TrySetMin(r, c + 1, d + 1),
                    'L' => TrySetMin(r - 1, c, d + 1) | TrySetMin(r, c + 1, d + 1),
                    'J' => TrySetMin(r - 1, c, d + 1) | TrySetMin(r, c - 1, d + 1),
                    '7' => TrySetMin(r + 1, c, d + 1) | TrySetMin(r, c - 1, d + 1),
                    'F' => TrySetMin(r + 1, c, d + 1) | TrySetMin(r, c + 1, d + 1),
                };
            }

            char GetMap(int r, int c)
            {
                return s == new Point2I(r, c) ? sl : data[r][c];
            }
        }

        protected override async Task ExecutePart2Async(string[] data)
        {
            await base.ExecutePart2Async(data);
        }
    }
}
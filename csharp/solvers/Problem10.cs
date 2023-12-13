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

            GPoint2I s = new();
            char sl = '\0';
            FixStart(data, cRows, cCols);
            long[,] dist = Algorithms.DistanceFill(data, s, p => GetMap(p.Row, p.Col) switch
            {
                '-' => new[]{p.Add(0, 1), p.Add(0, -1)},
                '|' => new[]{p.Add(1, 0), p.Add(-1, 0)},
                'F' => new[]{p.Add(0, 1), p.Add(1, 0)},
                'L' => new[]{p.Add(0, 1), p.Add(-1, 0)},
                '7' => new[]{p.Add(0, -1), p.Add(1, 0)},
                'J' => new[]{p.Add(0, -1), p.Add(-1, 0)},
            });

            long highest = dist.Cast<long>().Max();
            
            // There are only 2 possible configurations for a "piece" of the pipe that we'd see
            // in a row that are part of the loop, either a simple '|',
            // or a segment that looks like [FL]-*[7J]
            // We want to count the number of times we "cross" the pipe scanning each row
            // F--7 (and L--J) don't count, because they are just "bumps" we don't cross,
            // we are still on the same side of the polygon
            // F--J, L--7, and | all count as crossing from inside to outside (or the reverse)
            long insideCount = 0;
            for (int r = 0; r < cRows; r++)
            {
                bool inside = false;
                char bend = '-';
                for (int c = 0; c < cCols; c++)
                {
                    if (dist[r, c] == 0)
                    {
                        // It's some point that wasn't part of the loop! Count it (if it's inside)
                        if (inside)
                        {
                            insideCount++;
                            dist[r, c] = -1;
                        }
                    }
                    else
                    {
                        char cur = GetMap(r, c);
                        // Check to see if we crossed a "vertical" segment of pipe
                        if (cur == '|')
                            inside = !inside;
                        else if (cur == 'J')
                        {
                            // If we are at a J, then a "veritical" piece of pipe is
                            // "F--J", so only flip if we see the 'F'
                            if (bend == 'F')
                                inside = !inside;
                            bend = '-';
                        }
                        else if (cur == '7')
                        {
                            // only "L--7" is a bend
                            if (bend == 'L')
                                inside = !inside;
                            bend = '-';
                        }
                        // If it was F or L, we are at the beginning of a bend, save it to check at the end
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
                            Helpers.Verbose(".");
                        else if (dist[r,c] == -1)
                            Helpers.Verbose("\u2588");
                        else
                        {
                            char box = data[r][c] switch
                            {
                                '-' => '\u2550',
                                '|' => '\u2551',
                                'F' => '\u2554',
                                'L' => '\u255A',
                                '7' => '\u2557',
                                'J' => '\u255D',
                                var x => x,
                            };
                            Helpers.Verbose(box.ToString());
                            // Helpers.Verbose($"{dist[r, c] % 10} ");
                        }
                    }
                    Helpers.VerboseLine("");
                }
            }
            
            Console.WriteLine($"Furthest point is {highest}");
            Console.WriteLine($"Enclosed points {insideCount}");

            char GetMap(int r, int c)
            {
                return s == new GPoint2I(r, c) ? sl : data[r][c];
            }
        }

        private static void FixStart(string[] data, int cRows, int cCols)
        {
            GPoint2I s;
            char sl;
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
        }

        protected override async Task ExecutePart2Async(string[] data)
        {
            await base.ExecutePart2Async(data);
        }
    }
}
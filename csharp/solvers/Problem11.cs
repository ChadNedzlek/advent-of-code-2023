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
    public class Problem11 : DualAsyncProblemBase
    {
        protected override async Task ExecutePart1Async(string[] data)
        {
            int cRow = data.Length;
            int cCol = data[0].Length;
            (long[,] costs, HashSet<Point2I> galaxies) = InitializeCosts(data, cRow, cCol, 2);
            long total = 0;
            foreach (var g in galaxies)
            {
                foreach (var h in galaxies)
                {
                    if (g == h)
                        break;

                    long dist = 0;
                    int lr = g.Row, hr = h.Row;
                    int lc = g.Col, hc = h.Col;
                    if (lr > hr)
                    {
                        (hr, lr) = (lr, hr);
                    }

                    if (lc > hc)
                    {
                        (hc, lc) = (lc, hc);
                    }

                    for (int r = lr + 1; r <= hr; r++)
                    {
                        dist += costs[r, lc];
                    }
                    for (int c = lc + 1; c <= hc; c++)
                    {
                        dist += costs[lr, c];
                    }
                    Helpers.VerboseLine($"Sub Distance {dist}");
                    total += dist;

                }
            }
            Console.WriteLine($"Total distance : {total}");
        }

        private static (long[,] costs, HashSet<Point2I> galaxies) InitializeCosts(string[] data, int cRow, int cCol, int mult)
        {
            HashSet<Point2I> galaxies = new HashSet<Point2I>();
            long[,] costs = new long[cRow, cCol];
            for (int r = 0; r < cRow; r++)
            {
                bool empty = true;
                for (int c = 0; c < cCol; c++)
                {
                    if (data[r][c] != '.')
                    {
                        empty = false;
                        galaxies.Add(new Point2I(r, c));
                    }
                }
                for (int c = 0; c < cCol; c++)
                {
                    costs[r, c] = empty ? mult : 1;
                }
            }
            for (int c = 0; c < cCol; c++)
            {
                bool empty = true;
                for (int r = 0; r < cRow; r++)
                {
                    if (data[r][c] != '.')
                    {
                        empty = false;
                        break;
                    }
                }
                for (int r = 0; r < cRow; r++)
                {
                    costs[r, c] *= empty ? mult : 1;
                }
            }

            return (costs, galaxies);
        }

        protected override async Task ExecutePart2Async(string[] data)
        {
            int cRow = data.Length;
            int cCol = data[0].Length;
            (long[,] costs, HashSet<Point2I> galaxies) = InitializeCosts(data, cRow, cCol, 1_000_000);
            long total = 0;
            foreach (var g in galaxies)
            {
                foreach (var h in galaxies)
                {
                    if (g == h)
                        break;

                    long dist = 0;
                    int lr = g.Row, hr = h.Row;
                    int lc = g.Col, hc = h.Col;
                    if (lr > hr)
                    {
                        (hr, lr) = (lr, hr);
                    }

                    if (lc > hc)
                    {
                        (hc, lc) = (lc, hc);
                    }

                    for (int r = lr + 1; r <= hr; r++)
                    {
                        dist += costs[r, lc];
                    }
                    for (int c = lc + 1; c <= hc; c++)
                    {
                        dist += costs[lr, c];
                    }
                    Helpers.VerboseLine($"Sub Distance {dist}");
                    total += dist;

                }
            }
            Console.WriteLine($"Total distance : {total}");
        }
    }
}
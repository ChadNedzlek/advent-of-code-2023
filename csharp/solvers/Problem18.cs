using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ChadNedzlek.AdventOfCode.Library;
using Spectre.Console;

namespace ChadNedzlek.AdventOfCode.Y2023.CSharp.solvers
{
    public class Problem18 : DualAsyncProblemBase
    {
        protected override async Task ExecutePart1Async(string[] data)
        {
            Infinite2I<bool?> filled = new();
            int rCur = 0;
            int cCur = 0;
            foreach (var line in data)
            {
                var (dir, sLength, color) = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                int length = int.Parse(sLength);
                switch (dir)
                {
                    case "U":
                        for (int i = 0; i <= length; i++)
                        {
                            filled[rCur - i, cCur] = true;
                        }

                        rCur -= length;
                        break;
                    case "D":
                        for (int i = 0; i <= length; i++)
                        {
                            filled[rCur + i, cCur] = true;
                        }

                        rCur += length;
                        break;
                    case "R":
                        for (int i = 0; i <= length; i++)
                        {
                            filled[rCur, cCur + i] = true;
                        }

                        cCur += length;
                        break;
                    case "L":
                        for (int i = 0; i <= length; i++)
                        {
                            filled[rCur, cCur - i] = true;
                        }

                        cCur -= length;
                        break;
                }
            }

            int rMin = filled.GetLowerBound(0) - 1;
            int rMax = filled.GetUpperBound(0) + 1;
            int cMin = filled.GetLowerBound(1) - 1;
            int cMax = filled.GetUpperBound(1) + 1;

            filled[rMin, cMin] = default;
            filled[rMax, cMax] = default;

            Queue<(int r, int c)> toFill = new Queue<(int r, int c)>();
            toFill.Enqueue((rMin, cMin));
            while (toFill.TryDequeue(out var p))
            {
                if (filled.TryGet(p.r, p.c, out var v) && !v.HasValue)
                {
                    filled[p.r, p.c] = false;
                    toFill.Enqueue((p.r - 1, p.c));
                    toFill.Enqueue((p.r + 1, p.c));
                    toFill.Enqueue((p.r, p.c - 1));
                    toFill.Enqueue((p.r, p.c + 1));
                }
            }

            for (int r = rMin; r <= rMax; r++)
            {
                for (int c = cMin; c <= cMax; c++)
                {
                    var v = filled[r, c];
                    Helpers.Verbose(v.HasValue ? v.Value ? "#" : "." : " ");
                }

                Helpers.VerboseLine("");
            }

            var cAll = filled.ToArray().Count();
            var piles = filled.CountBy(v => v.HasValue ? v.Value ? "#" : "." : " ");
            var cFilled = filled.ToArray().Count(x => x ?? true);
            Console.WriteLine($"Filled = {cFilled}");
        }

        protected override async Task ExecutePart2Async(string[] data)
        {
            // The algorithm here is... weird.
            // Step 1 is "pretend the lines don't have any thickness"
            // Which is a pretty easy "add space under line when going right, subtract space under the line going left"
            // Then you need to account for the "thickness" of the lines.  If we fatten 'em up,
            // half the "area" of the line is inside the box already, so we add up all the lines and divide by two
            // And, then imagine the "zero length" square. It's still area "1".  The four corners of that 1
            // expand outward when the size isn't zero anymore... so plus one
            long rCur = 0;
            long cCur = 0;
            long distSum = 0;
            long lineWeights = 0;
            foreach (var line in data)
            {
                var (_, _, inst) = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                long length = int.Parse(inst[2..7], NumberStyles.HexNumber);
                string dir = inst[7..8];
                lineWeights += length;
                switch (dir)
                {
                    case "3": // up
                        rCur -= length;
                        break;
                    case "1": // down
                        rCur += length;
                        break;
                    case "0": // right
                    {
                        distSum -= rCur * length;
                        cCur += length;
                    }
                        break;
                    case "2": // left
                    {
                        distSum += rCur * length;
                        cCur -= length;
                        break;
                    }
                }
            }

            Console.WriteLine($"A value? {Math.Abs(distSum)} + {lineWeights} / 2 + 1 = {Math.Abs(distSum) + lineWeights/2 + 1}");
        }
    }
}
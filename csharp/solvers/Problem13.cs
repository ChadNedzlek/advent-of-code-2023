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
    public class Problem13 : DualAsyncProblemBase
    {
        protected override async Task ExecutePart1Async(string[] data)
        {
            DoPart(data, 0);
        }

        private static void DoPart(string[] data, int target)
        {
            long rSum = 0;
            long cSum = 0;
            var parts = data.SplitWhen(string.IsNullOrWhiteSpace).Select(p => p.ToArray());
            foreach (var part in parts)
            {
                for (int r = 0; r < part.Length - 1; r++)
                {
                    if (IsValidVerticalReflection(part, r, target))
                    {
                        rSum += r + 1;
                    }
                }

                for (int c = 0; c < part[0].Length - 1; c++)
                {
                    if (IsValidHorizontalReflection(part, c, target))
                    {
                        cSum += c + 1;
                    }
                }
            }

            Console.WriteLine($"Pointless value = 100 * {rSum} + {cSum} = {100 * rSum + cSum}");
        }

        private static bool IsValidVerticalReflection(string[] data, int refR, int targetCount)
        {
            int count = 0;
            for (int or = 0; or < data.Length; or++)
            {
                for (int c = 0; c < data[0].Length; c++)
                {
                    if (data.TryGet(refR - or, c, out var a) && data.TryGet(refR + or + 1, c, out var b) && a != b)
                    {
                        count++;
                        if (count > targetCount)
                            return false;
                    }
                }
            }

            return count == targetCount;
        }
        
        private static bool IsValidHorizontalReflection(string[] data, int refC, int targetCount)
        {
            int count = 0;
            for (int oc = 0; oc < data.Length; oc++)
            {
                for (int r = 0; r < data.Length; r++)
                {
                    if (data.TryGet(r, refC - oc, out var a) && data.TryGet(r, refC + oc + 1, out var b) && a != b)
                    {
                        count++;
                        if (count > targetCount)
                            return false;
                    }
                }
            }

            return count == targetCount;
        }

        protected override async Task ExecutePart2Async(string[] data)
        {
            DoPart(data, 1);
        }
    }
}
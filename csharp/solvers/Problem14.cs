using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChadNedzlek.AdventOfCode.Library;

namespace ChadNedzlek.AdventOfCode.Y2023.CSharp.solvers
{
    public class Problem14 : DualAsyncProblemBase
    {
        protected override async Task ExecutePart1Async(string[] data)
        {
            long totalLoad = 0;
            for (int c = 0; c < data[0].Length; c++)
            {
                long load = data.Length;
                for (int r = 0; r < data.Length; r++)
                {
                    if (data[r][c] == 'O')
                    {
                        totalLoad += (load--);
                    } else if (data[r][c] == '#')
                    {
                        load = data.Length - r - 1;
                    }
                }
            }
            Console.WriteLine($"Total load {totalLoad}");
        }

        protected override async Task ExecutePart2Async(string[] data)
        {
            int nRows = data.Length;
            int nCols = data[0].Length;
            char[,] map = new char[nRows, nCols];
            for (var r = 0; r < nRows; r++)
            {
                for (var c = 0; c < nCols; c++)
                {
                    map[r,c] = data[r][c];
                }
            }

            int moves = 1000000000;
            Dictionary<string, (int when, int load)> cachedLoads = new();
            for (int i = 0; i < moves; i++)
            {
                bool moved = true;
                while (moved)
                {
                    moved = false;
                    for (var r = 1; r < nRows; r++)
                    {
                        for (var c = 0; c < nCols; c++)
                        {
                            if (map[r, c] == 'O' && map[r - 1, c] == '.')
                            {
                                map[r, c] = '.';
                                map[r - 1, c] = 'O';
                                moved = true;
                            }
                        }
                    }
                }

                moved = true;
                while (moved)
                {
                    moved = false;
                    for (var r = 0; r < nRows; r++)
                    {
                        for (var c = 1; c < nCols; c++)
                        {
                            if (map[r, c] == 'O' && map[r, c - 1] == '.')
                            {
                                map[r, c] = '.';
                                map[r, c - 1] = 'O';
                                moved = true;
                            }
                        }
                    }
                }

                moved = true;
                while (moved)
                {
                    moved = false;
                    for (var r = 0; r < nRows - 1; r++)
                    {
                        for (var c = 0; c < nCols; c++)
                        {
                            if (map[r, c] == 'O' && map[r + 1, c] == '.')
                            {
                                map[r, c] = '.';
                                map[r + 1, c] = 'O';
                                moved = true;
                            }
                        }
                    }
                }

                moved = true;
                while (moved)
                {
                    moved = false;
                    for (var r = 0; r < nRows; r++)
                    {
                        for (var c = 0; c < nCols - 1; c++)
                        {
                            if (map[r, c] == 'O' && map[r, c + 1] == '.')
                            {
                                map[r, c] = '.';
                                map[r, c + 1] = 'O';
                                moved = true;
                            }
                        }
                    }
                }
                
                int totalLoad = 0;
                for (var r = 0; r < nRows; r++)
                {
                    for (var c = 0; c < nCols; c++)
                    {
                        if (map[r,c] == 'O')
                        {
                            totalLoad += nRows - r;
                        }
                    }
                }

                string sig = CalculateSignature(map);
                if (cachedLoads.TryGetValue(sig, out var old))
                {
                    var cycle = i - old.when;
                    var remCycles = (moves - i - 1) % cycle;
                    var endRep = cachedLoads.Values.First(v => v.when == old.when + remCycles);
                    var after = cachedLoads.Values.First(v => v.when == old.when + remCycles + 1);
                    var before = cachedLoads.Values.First(v => v.when == old.when + remCycles - 1);
                    Helpers.VerboseLine($"During the {i} cycle == {old.when} with {old.load}, meaning the final is {remCycles} into the cycle at {endRep.when} with a load of {endRep.load}");
                    Console.WriteLine($"Final values is {endRep.load}");
                    return;
                }

                cachedLoads.Add(sig, (i, totalLoad));
            }
        }

        private void DumpMap(char[,] map)
        {
            Console.WriteLine();
            for (var r = 0; r < map.GetLength(0); r++)
            {
                for (var c = 0; c < map.GetLength(1); c++)
                {
                    Console.Write(map[r,c]);
                }
                Console.WriteLine();
            }
        }

        private string CalculateSignature(char[,] map)
        {
            StringBuilder b = new StringBuilder();
            for (var r = 0; r < map.GetLength(0); r++)
            {
                for (var c = 0; c < map.GetLength(1); c++)
                {
                    b.Append(map[r,c]);
                }
                b.Append('\n');
            }

            return b.ToString();
        }
    }
}
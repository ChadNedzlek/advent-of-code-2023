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
    public class Problem02 : DualAsyncProblemBase
    {
        protected override async Task ExecutePart1Async(string[] data)
        {
            Console.WriteLine("AI version: " + CheckGamePossibility(data, 12, 13, 14));
            
            Dictionary<string, int> counts = new()
            {
                ["red"] = 12,
                ["green"] = 13,
                ["blue"] = 14,
            };
            long total = 0;
            foreach (string[] line in data.Select(d => d.Split(':')))
            {
                long id = long.Parse(line[0].Split(' ')[1]);
                bool valid = IsValidGame(line, counts);

                if (valid)
                {
                    total += id;
                    Console.WriteLine($"Id {id} is a valid game");
                }
            }

            Console.WriteLine($"Total = {total}");
        }

        public static int CheckGamePossibility(string[] games, int redCube, int greenCube, int blueCube)
        {
            int total = 0;
            foreach (var game in games)
            {
                int maxRed = 0, maxBlue = 0, maxGreen = 0;
                var gameData = game.Split(':');
                int gameId = int.Parse(gameData[0].Split(' ')[1]);

                var gameRounds = gameData[1].Split(';');
                foreach (var round in gameRounds)
                {
                    var matches = Regex.Matches(round, @"(\d+) (red|blue|green)");
                    foreach (Match match in matches)
                    {
                        int number = int.Parse(match.Groups[1].Value);
                        string color = match.Groups[2].Value;
                        if (color == "red") maxRed = Math.Max(maxRed, number);
                        if (color == "green") maxGreen = Math.Max(maxGreen, number);
                        if (color == "blue") maxBlue = Math.Max(maxBlue, number);
                    }
                }

                if (maxRed <= redCube && maxGreen <= greenCube && maxBlue <= blueCube)
                {
                    total += gameId;
                }
            }
            return total;
        }

        protected async Task ExecutePart5Async(IAsyncEnumerable<string> data)
        {
            long total = 0;
            await foreach (string[] line in data.Select(d => d.Split(':')))
            {
                long id = long.Parse(line[0].Split(' ')[1]);
                Dictionary<string, long> min = new();
                foreach (string[] draw in line[1].Split(';').Select(s => s.Split(',')))
                {
                    foreach (var part in draw)
                    {
                        var die = part.Trim().Split(' ');
                        string name = die[1];
                        long count = long.Parse(die[0]);
                        min.AddOrUpdate(name, count, c => Math.Max(c, count));
                    }
                }

                var power = min.Values.Aggregate(1L, (a, b) => a * b);
                Console.WriteLine($"Game {id} power = {power} [{string.Join(",", min.Select(m => $"{m.Value} {m.Key}"))}]");
                total += power;
            }

            Console.WriteLine($"Total = {total}");
        }

        private static bool IsValidGame(string[] line, Dictionary<string, int> counts)
        {

            return true;
        }
    }
}
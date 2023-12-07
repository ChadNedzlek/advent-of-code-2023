using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ChadNedzlek.AdventOfCode.Library;
using Spectre.Console;

namespace ChadNedzlek.AdventOfCode.Y2023.CSharp.solvers
{
    public class Problem07 : DualAsyncProblemBase
    {
        protected override async Task ExecutePart1Async(IAsyncEnumerable<string> data)
        {
            var list = await data.Select(d => d.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                .Select(a => new Hand(a[0], long.Parse(a[1]), false)).OrderBy(x => x).ToListAsync();
            var values = list.Select((h, i) => (Hand: h, Total: h.Bid * (i + 1))).ToList();
            if (Helpers.IncludeVerboseOutput)
            {
                for (var rank = 0; rank < values.Count; rank++)
                {
                    (Hand Hand, long Total) = values[rank];
                    Console.WriteLine($"Hand {Hand.Values} [{Hand.Category}] bid {Hand.Bid} at rank {rank+1} for total {Total}");
                }
            }

            var total = values.Sum(v => v.Total);
            Console.WriteLine($"Sum: {total}");
        }

        protected override async Task ExecutePart2Async(IAsyncEnumerable<string> data)
        {
            var list = await data.Select(d => d.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                .Select(a => new Hand(a[0], long.Parse(a[1]), true)).OrderBy(x => x).ToListAsync();
            var values = list.Select((h, i) => (Hand: h, Total: h.Bid * (i + 1))).ToList();
            if (Helpers.IncludeVerboseOutput)
            {
                for (var rank = 0; rank < values.Count; rank++)
                {
                    (Hand Hand, long Total) = values[rank];
                    Console.WriteLine($"Hand {Hand.Values} [{Hand.Category}] bid {Hand.Bid} at rank {rank+1} for total {Total}");
                }
            }

            var total = values.Sum(v => v.Total);
            Console.WriteLine($"Sum: {total}");
        }

        public enum HandRank
        {
            HighCard,
            Pair,
            TwoPair,
            ThreeOfAKind,
            FullHouse,
            FourOfAKind,
            FiveOfAKind,
        }

        public record struct Hand(string Values, long Bid, bool Wild) : IComparable<Hand>
        {
            private static readonly ImmutableList<char> Letters = "23456789TQKA".ToImmutableList();

            private string GetBestWildHand()
            {
                if (!Wild)
                    return Values;

                string best = Values;
                HandRank bestCat = CalculateCategory(best);
                string f = Values.Replace("J", "");
                if (f.Length == Values.Length)
                    return Values;

                foreach (var perm in Algorithms.Permute(Letters, Values.Length - f.Length))
                {
                    string permHand = f + new string((ReadOnlySpan<char>)perm);
                    var permCat = CalculateCategory(permHand);
                    if (permCat > bestCat)
                    {
                        best = permHand;
                        bestCat = permCat;
                    }
                }

                return best;
            }

            private ImmutableList<int> _matches;
            public ImmutableList<int> Matches {
                get
                {
                    if (_matches == null)
                    {
                        _matches = CalculateMatches(Values);
                    }

                    return _matches;
                }
            }

            private HandRank? _category;
            public HandRank Category
            {
                get
                {
                    if (!_category.HasValue)
                    {
                        if (Wild)
                        {
                            _category = CalculateCategory(GetBestWildHand());
                        }
                        else
                        {
                            _category = CalculateCategory(Matches);
                        }
                    }

                    return _category.Value;
                }
            }


            private static ImmutableList<int> CalculateMatches(string values)
            {
                return values.GroupBy(x => x).Select(g => g.Count()).OrderDescending().ToImmutableList();
            }
            private static HandRank CalculateCategory(ImmutableList<int> matches)
            {
                return matches[0] switch
                {
                    1 => HandRank.HighCard,
                    2 => matches[1] == 2 ? HandRank.TwoPair : HandRank.Pair,
                    3 => matches[1] == 2 ? HandRank.FullHouse : HandRank.ThreeOfAKind,
                    4 => HandRank.FourOfAKind,
                    5 => HandRank.FiveOfAKind,
                };
            }

            private static HandRank CalculateCategory(string value) => CalculateCategory(CalculateMatches(value));

            public int CompareTo(Hand other)
            {
                var cat = Category;
                var oCat = other.Category;
                if (cat != oCat)
                    return cat.CompareTo(oCat);
                

                for (int i = 0; i < Values.Length; i++)
                {
                    int a = Rank(Values[i]);
                    int b = Rank(other.Values[i]);
                    if (a != b)
                        return a.CompareTo(b);
                }

                throw new ArgumentException();
            }

            public int Rank(char c)
            {
                if (char.IsDigit(c))
                    return c - '0';
                return c switch
                {
                    'T' => 10,
                    'J' => Wild ? 1 : 11,
                    'Q' => 12,
                    'K' => 13,
                    'A' => 14,
                };
            }
        }
    }
}
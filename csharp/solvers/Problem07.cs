using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using ChadNedzlek.AdventOfCode.Library;

namespace ChadNedzlek.AdventOfCode.Y2023.CSharp.solvers
{
    public class Problem07 : DualAsyncProblemBase
    {
        protected override async Task ExecutePart1Async(IAsyncEnumerable<string> data)
        {
            await CalculateHands(data, false);
        }

        protected override async Task ExecutePart2Async(IAsyncEnumerable<string> data)
        {
            await CalculateHands(data, true);
        }

        private static async Task CalculateHands(IAsyncEnumerable<string> data, bool wilds)
        {
            var list = await data.Select(d => d.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                .Select(a => new Hand(a[0], long.Parse(a[1]), wilds)).OrderBy(x => x).ToListAsync();
            var values = list.Select((h, i) => (Hand: h, Total: h.Bid * (i + 1))).ToList();
            if (Helpers.IncludeVerboseOutput)
            {
                for (var rank = 0; rank < values.Count; rank++)
                {
                    (Hand Hand, long Total) = values[rank];
                    Console.WriteLine(
                        $"Hand {Hand.Values} [{Hand.Category}] bid {Hand.Bid} at rank {rank + 1} for total {Total}");
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
            private string GetBestWildHand()
            {
                if (!Wild)
                    return Values;

                string best = Values;
                HandRank bestCat = CalculateCategory(best);
                string f = Values.Replace("J", "");
                // We only need to try the cards in the hand, since only sets count
                List<char> inHand = f.Distinct().ToList();
                if (f.Length == Values.Length || f.Length == 0)
                    return Values;

                foreach (var perm in Algorithms.Permute(inHand, Values.Length - f.Length))
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
                return matches switch
                {
                    [5] => HandRank.FiveOfAKind,
                    [4,1] => HandRank.FourOfAKind,
                    [3,2] => HandRank.FullHouse,
                    [3,1,1] => HandRank.ThreeOfAKind,
                    [2,2,1] => HandRank.TwoPair,
                    [2,1,1,1] => HandRank.Pair,
                    _ => HandRank.HighCard
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
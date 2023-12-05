using System.Numerics;

namespace ChadNedzlek.AdventOfCode.Library;

public readonly record struct Rect2<T>(T Left, T Top, T Right, T Bottom)
    where T : IAdditionOperators<T, T, T>, IComparisonOperators<T, T, bool>
{
    public bool IsInBounds(Point2<T> p)
    {
        return p.X >= Left && p.X <= Right && p.Y >= Top && p.Y <= Bottom;
    }
}
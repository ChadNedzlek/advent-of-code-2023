using System.Numerics;

namespace ChadNedzlek.AdventOfCode.Library;

public readonly record struct Rect2(int Left, int Top, int Right, int Bottom)
{
    public bool IsInBounds(Point2L p)
    {
        return p.X >= Left && p.X <= Right && p.Y >= Top && p.Y <= Bottom;
    }
}

public readonly record struct Rect2L(long Left, long Top, long Right, long Bottom)
{
    public bool IsInBounds(Point2L p)
    {
        return p.X >= Left && p.X <= Right && p.Y >= Top && p.Y <= Bottom;
    }
}
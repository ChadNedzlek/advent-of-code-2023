using System.Numerics;

namespace ChadNedzlek.AdventOfCode.Library;

public readonly record struct Point2L(long X, long Y) : IConvertable<(long x, long y), Point2L>
{
    public static implicit operator Point2L((long x, long y) p) => new(p.x, p.y);

    public Point2L Add(Point2L d)
    {
        return new Point2L(d.X + X, d.Y + Y);
    }

    public Point2L Add(long dx, long dy)
    {
        return new Point2L(dx + X, dy + Y);
    }

    public bool Equals(long x, long y)
    {
        return X == x && Y == y;
    }
}

public readonly record struct Point2I(int X, int Y) : IConvertable<(int x, int y), Point2I>
{
    public static implicit operator Point2I((int x, int y) p) => new(p.x, p.y);

    public Point2I Add(Point2I d)
    {
        return new Point2I(d.X + X, d.Y + Y);
    }

    public Point2I Add(int dx, int dy)
    {
        return new Point2I(dx + X, dy + Y);
    }

    public bool Equals(int x, int y)
    {
        return X == x && Y == y;
    }
}
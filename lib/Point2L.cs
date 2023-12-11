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

public readonly record struct Point2I(int Row, int Col) : IConvertable<(int row, int col), Point2I>
{
    public static implicit operator Point2I((int row, int col) p) => new(p.row, p.col);

    public Point2I Add(Point2I d)
    {
        return new Point2I(d.Row + Row, d.Col + Col);
    }

    public Point2I Add(int dx, int dy)
    {
        return new Point2I(dx + Row, dy + Col);
    }

    public bool Equals(int x, int y)
    {
        return Row == x && Col == y;
    }
}
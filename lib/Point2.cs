using System.Numerics;

namespace ChadNedzlek.AdventOfCode.Library;

public readonly record struct Point2<T>(T X, T Y) : IConvertable<(T x, T y), Point2<T>>
    where T : IAdditionOperators<T, T, T>, IEqualityOperators<T, T, bool>
{
    public static implicit operator Point2<T>((T x, T y) p) => new(p.x, p.y);

    public Point2<T> Add(Point2<T> d)
    {
        return new Point2<T>(d.X + X, d.Y + Y);
    }

    public Point2<T> Add(T dx, T dy)
    {
        return new Point2<T>(dx + X, dy + Y);
    }

    public bool Equals(T x, T y)
    {
        return X == x && Y == y;
    }
}
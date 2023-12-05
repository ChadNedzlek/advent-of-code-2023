using System.Numerics;

namespace ChadNedzlek.AdventOfCode.Library;

public readonly record struct Point3<T>(T X, T Y, T Z) : IConvertable<(T x,T y, T z), Point3<T>>
    where T:IAdditionOperators<T,T,T>
{
    public static implicit operator Point3<T>((T x, T y, T z) p) => new(p.x, p.y, p.z);

    public Point3<T> Add(Point3<T> d)
    {
        return new Point3<T>(d.X + X, d.Y + Y, d.Z + Z);
    }
        
    public Point3<T> Add(T dx, T dy, T dz)
    {
        return new Point3<T>(dx + X, dy + Y, dz + Z);
    }
}
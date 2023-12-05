using System.Collections.Generic;
using System.Numerics;

namespace ChadNedzlek.AdventOfCode.Y2023.CSharp
{
    public record struct LRange(long Start, long Length)
    {
        public long End => Start + Length;

        public override string ToString() => $"{Start}-{End}";

        public void SpliceOut(LRange other, out LRange? before, out LRange? mid, out LRange? after)
        {
            if (End < other.Start || Start >= other.End)
            {
                before = this;
                mid = after = null;
                return;
            }

            before = after = null;
            LRange cur = this;
            if (cur.Start < other.Start)
            {
                before = cur with { Length = other.Start - cur.Start };
                cur = new LRange(Start: other.Start, Length: cur.End - other.Start);
            }
            
            if (cur.End > other.End)
            {
                after = new LRange(other.End, cur.End - other.End);
                mid = cur with { Length = other.End - cur.Start };
            }

            mid = cur;
        }
    }

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

    public interface IConvertable<in T1, out T2> where T2 : IConvertable<T1, T2>
    {
        static abstract implicit operator T2(T1 p);
    }

    public readonly record struct Rect2<T>(T Left, T Top, T Right, T Bottom)
        where T : IAdditionOperators<T, T, T>, IComparisonOperators<T, T, bool>
    {
        public bool IsInBounds(Point2<T> p)
        {
            return p.X >= Left && p.X <= Right && p.Y >= Top && p.Y <= Bottom;
        }
    }
}
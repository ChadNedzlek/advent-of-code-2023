namespace ChadNedzlek.AdventOfCode.Library;

public readonly record struct RangeL(long Start, long Length)
{
    /// <summary>
    /// Exclusive end boundary.
    /// </summary>
    /// <example>{Start:1, End:4} => [1, 2, 3]</example>
    public long End => Start + Length;

    public override string ToString() => $"{Start}-{End}";

    public void SpliceOut(RangeL other, out RangeL? before, out RangeL? mid, out RangeL? after)
    {
        if (End < other.Start || Start >= other.End)
        {
            before = this;
            mid = after = null;
            return;
        }

        before = after = null;
        RangeL cur = this;
        if (cur.Start < other.Start)
        {
            before = cur with { Length = other.Start - cur.Start };
            cur = other with { Length = cur.End - other.Start };
        }
            
        if (cur.End > other.End)
        {
            after = new RangeL(other.End, cur.End - other.End);
            mid = cur with { Length = other.End - cur.Start };
        }

        mid = cur;
    }

    public bool Contains(long value) => Start <= value && value < End;
}
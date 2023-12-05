namespace ChadNedzlek.AdventOfCode.Library;

public readonly record struct LRange(long Start, long Length)
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
            cur = other with { Length = cur.End - other.Start };
        }
            
        if (cur.End > other.End)
        {
            after = new LRange(other.End, cur.End - other.End);
            mid = cur with { Length = other.End - cur.Start };
        }

        mid = cur;
    }
}
using System.Collections;

namespace KirisameLib.Core.Collections;

public class NumRange<T>(int from, int to, Func<int, T> selector) : IReadOnlyCollection<T>
{
    public T Head { get; } = selector(from);
    public T Tail { get; } = selector(to);
    private IntRange IntRange { get; } = new(from, to);

    public IEnumerator<T> GetEnumerator()
    {
        return IntRange.Select(selector).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int Count => IntRange.Count;
}

public class IntRange(int from, int to) : IReadOnlyCollection<int>
{
    public int Head { get; } = from;
    public int Tail { get; } = to;

    public IEnumerator<int> GetEnumerator()
    {
        if (Head < Tail)
        {
            for (int i = Head; i < Tail; i++)
                yield return i;
        }
        else
        {
            for (int i = Head - 1; i >= Tail; i++)
                yield return i;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int Count => Math.Abs(Tail - Head);
}
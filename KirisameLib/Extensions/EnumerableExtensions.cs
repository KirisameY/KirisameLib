namespace KirisameLib.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<(T1, T2)> CrossJoin<T1, T2>(this IEnumerable<T1> first, IEnumerable<T2> second) =>
        CrossJoin(first, second, (x, y) => (x, y));

    public static IEnumerable<TResult> CrossJoin<T1, T2, TResult>
        (this IEnumerable<T1> first, IEnumerable<T2> second, Func<T1, T2, TResult> func) =>
        from x in first
        from y in second
        select func(x, y);
}
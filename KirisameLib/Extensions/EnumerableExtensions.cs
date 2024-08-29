namespace KirisameLib.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<(T1, T2)> CrossJoin<T1, T2>(this IEnumerable<T1> first, IEnumerable<T2> second) =>
        from x in first
        from y in second
        select (x, y);
}
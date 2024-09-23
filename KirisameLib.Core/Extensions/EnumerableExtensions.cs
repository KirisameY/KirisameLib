namespace KirisameLib.Core.Extensions;

public static class EnumerableExtensions
{
    //single
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        foreach (var element in enumerable)
        {
            action(element);
        }
    }

    public static int PositionOfFirst<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        int order = 0;
        foreach (var item in source)
        {
            if (predicate(item)) return order;
            order++;
        }
        return -1;
    }

    public static int PositionOfFirst<T>(this IEnumerable<T> source, T target) => PositionOfFirst(source, item => Equals(item, target));

    public static IEnumerable<TResult> SelectExist<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult?> selector) =>
        source.Select(selector).OfType<TResult>();

    //dual
    public static IEnumerable<(TFirst, TSecond)> CrossJoin<TFirst, TSecond>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second) =>
        CrossJoin(first, second, (x, y) => (x, y));

    public static IEnumerable<TResult> CrossJoin<TFirst, TSecond, TResult>
        (this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector) =>
        from x in first
        from y in second
        select resultSelector(x, y);
}
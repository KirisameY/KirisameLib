using System;
using System.Collections.Generic;
using System.Linq;

namespace KirisameLib.GeneratorTools.Extensions;

public static class LinqExtensions
{
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) => source.OfType<T>();

    public static IEnumerable<TResult> SelectNotNull<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult?> selector)
    {
        return source.Select(selector).OfType<TResult>();
    }
}
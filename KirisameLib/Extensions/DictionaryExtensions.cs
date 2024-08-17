namespace KirisameLib.Extensions;

public static class DictionaryExtensions
{
    public static TValue? GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue? defaultValue = default)
        where TKey : notnull
    {
        return dict.TryGetValue(key, out var value) ? value : defaultValue;
    }
}
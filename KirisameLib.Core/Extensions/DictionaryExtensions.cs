namespace KirisameLib.Core.Extensions;

public static class DictionaryExtensions
{
    [Obsolete("use GetValueOrDefault instead")]
    public static TValue? GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue? defaultValue = default)
    {
        return dict.TryGetValue(key, out var value) ? value : defaultValue;
    }
}
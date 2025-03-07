using System.Collections.Frozen;

namespace KirisameLib.Data.Registers;

public class FrozenRegister<T>(IDictionary<string, T> regDict, Func<string, T> fallback) : IRegister<T>
{
    private readonly FrozenDictionary<string, T> _regDict = regDict.ToFrozenDictionary();

    public T this[string id] => GetItem(id);

    public T GetItem(string id)
    {
        if (!_regDict.TryGetValue(id, out var value))
        {
            try { value = fallback(id); }
            catch (Exception e)
            {
                throw new GettingFallbackValueFailedException(id, e);
            }
        }

        return value;
    }

    public bool ItemRegistered(string id) => _regDict.ContainsKey(id);
}
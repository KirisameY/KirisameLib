using System.Collections;
using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;

using KirisameLib.Data.Registering;

namespace KirisameLib.Data.Registers;

/// <summary>
///     An immutable register.<br/>
///     The most common implementation of <see cref="IRegister{TItem}"/>.
/// </summary>
/// <param name="regDict">
///     Source dictionary that contains all registered id-item pairs.<br/>
///     Will not be stored in the new instance.
/// </param>
/// <param name="fallback"> Fallback function for items that are not registered. </param>
/// <seealso cref="MoltenRegister{TItem}"/>
/// <seealso cref="RegisterBuilder{TItem}"/>
public class FrozenRegister<TItem>(IDictionary<string, TItem> regDict, Func<string, TItem> fallback) : IEnumerableRegister<TItem>
{
    private readonly FrozenDictionary<string, TItem> _regDict = regDict.ToFrozenDictionary();

    public TItem this[string id] => GetItem(id);
    public IEnumerable<string> Keys => _regDict.Keys;
    public IEnumerable<TItem> Values => _regDict.Values;
    public int Count => _regDict.Count;

    public TItem GetItem(string id)
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

    public bool TryGetValue(string key, out TItem value)
    {
        bool result = _regDict.TryGetValue(key, out var item);
        value = result ? item! : fallback(key);
        return result;
    }

    public IEnumerator<KeyValuePair<string, TItem>> GetEnumerator() => _regDict.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
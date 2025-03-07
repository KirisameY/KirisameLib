using System.Collections.Frozen;

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
public class FrozenRegister<TItem>(IDictionary<string, TItem> regDict, Func<string, TItem> fallback) : IRegister<TItem>
{
    private readonly FrozenDictionary<string, TItem> _regDict = regDict.ToFrozenDictionary();

    public TItem this[string id] => GetItem(id);

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
}
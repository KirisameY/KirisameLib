using KirisameLib.Data.Registering;

namespace KirisameLib.Data.Registers;

/// <summary>
///     MoltenRegister, i.e. common register before freezing.<br/>
///     Allows for registration &amp; alteration of items, and can be converted to a <see cref="FrozenRegister{TItem}"/>.<br/>
///     It is recommended to use it as an intermediate object for creating a frozen registry.
/// </summary>
/// <param name="fallback"> Fallback function for items that are not registered </param>
/// <remarks>
///     Note that this register cannot be used after freezing.
/// </remarks>
/// <seealso cref="RegisterBuilder{TItem}"/>
public class MoltenRegister<TItem>(Func<string, TItem> fallback) : IRegister<TItem>
{
    private bool _frozen = false;

    private readonly Dictionary<string, TItem> _regDict = new();

    /// <summary>
    ///     Try to add an item to the register.
    /// </summary>
    /// <returns> Whether the item is added successfully. </returns>
    /// <exception cref="RegisterAlreadyFrozenException"> Register is already frozen. </exception>
    public bool AddItem(string id, TItem item)
    {
        if (_frozen) throw new RegisterAlreadyFrozenException();
        return _regDict.TryAdd(id, item);
    }

    /// <summary>
    ///     Add an item to the register, if already exists, overwrite it.
    /// </summary>
    /// <exception cref="RegisterAlreadyFrozenException"> Register is already frozen. </exception>
    public void AddOrOverwriteItem(string id, TItem item)
    {
        if (_frozen) throw new RegisterAlreadyFrozenException();
        _regDict[id] = item;
    }

    public TItem GetItem(string id)
    {
        if (_frozen) throw new RegisterAlreadyFrozenException();
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

    public bool ItemRegistered(string id)
    {
        if (_frozen) throw new RegisterAlreadyFrozenException();
        return _regDict.ContainsKey(id);
    }


    /// <summary>
    ///     Freeze current molten register to a frozen register.
    /// </summary>
    /// <returns> The new FrozenRegister. </returns>
    /// <exception cref="RegisterAlreadyFrozenException"> Register is already frozen. </exception>
    /// <remarks>
    ///     The frozen register will be a new instance, and after freezing this instance will no more available and will throw an exception when used<br/>
    ///     Be sure to save the return value and discard this reference.
    /// </remarks>
    public FrozenRegister<TItem> Freeze()
    {
        if (_frozen) throw new RegisterAlreadyFrozenException();
        _frozen = true;
        return new FrozenRegister<TItem>(_regDict, fallback);
    }


}

public class RegisterAlreadyFrozenException() : InvalidOperationException("Try to visit a frozen MoltenRegister.");
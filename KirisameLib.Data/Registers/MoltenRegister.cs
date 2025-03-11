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
public class MoltenRegister<TItem>(Func<string, TItem> fallback) : IRegTarget<TItem>
{
    private bool _frozen = false;

    private readonly Dictionary<string, TItem> _regDict = new();

    public bool AvailableToReg => !_frozen;

    public bool AddItem(string id, TItem item)
    {
        if (_frozen) throw new RegisterTargetUnavailableException();
        return _regDict.TryAdd(id, item);
    }

    public void AddOrOverwriteItem(string id, TItem item)
    {
        if (_frozen) throw new RegisterTargetUnavailableException();
        _regDict[id] = item;
    }

    public TItem GetItem(string id)
    {
        if (_frozen) throw new RegisterTargetUnavailableException();
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
        if (_frozen) throw new RegisterTargetUnavailableException();
        return _regDict.ContainsKey(id);
    }


    /// <summary>
    ///     Freeze current molten register to a frozen register.
    /// </summary>
    /// <returns> The new FrozenRegister. </returns>
    /// <exception cref="RegisterTargetUnavailableException"> Register is already frozen. </exception>
    /// <remarks>
    ///     The frozen register will be a new instance, and after freezing this instance will no more available and will throw an exception when used<br/>
    ///     Be sure to save the return value and discard this reference.
    /// </remarks>
    public FrozenRegister<TItem> Freeze()
    {
        if (_frozen) throw new RegisterTargetUnavailableException();
        _frozen = true;
        return new FrozenRegister<TItem>(_regDict, fallback);
    }
}
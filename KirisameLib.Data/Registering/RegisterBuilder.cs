using KirisameLib.Data.Registers;

namespace KirisameLib.Data.Registering;

/// <summary>
///     Builder for create an immutable <see cref="IRegister{TItem}"/>, which implemented by <see cref="FrozenRegister{TItem}"/> <br/>
///     Work with <see cref="IRegistrant{TItem}"/>, will freeze inner <see cref="MoltenRegister{TItem}"/>
///     when the registrant finished work and allow external access through the returned proxy object.
/// </summary>
/// <remarks> A fallback is necessary to build a register. </remarks>
public class RegisterBuilder<TItem>
{
    private Func<string, TItem>? _fallback;

    private readonly HashSet<IRegistrant<TItem>> _registrants = [];

    /// <summary>
    ///     Set a default item as fallback.<br/>
    ///     One of overloads of this method is necessary to build a register.
    /// </summary>
    public RegisterBuilder<TItem> WithFallback(TItem fallback) => WithFallback(_ => fallback);

    /// <summary>
    ///     Set a fallback function for items that are not registered.<br/>
    ///     One of overloads of this method is necessary to build a register.
    /// </summary>
    public RegisterBuilder<TItem> WithFallback(Func<string, TItem> fallback)
    {
        _fallback = fallback;
        return this;
    }

    /// <summary>
    ///     Add a registrant for registering.<br/>
    ///     After all added registrants are done, the created register will be frozen and available.
    /// </summary>
    public RegisterBuilder<TItem> WithRegistrant(IRegistrant<TItem> registrant)
    {
        _registrants.Add(registrant);
        return this;
    }

    /// <summary>
    ///     Build a register with current settings.
    /// </summary>
    /// <returns>The created register, will available after all added registrants are done.</returns>
    /// <exception cref="InvalidOperationException"> Fallback is not set. </exception>
    public IRegister<TItem> Build()
    {
        if (_fallback is null) throw new InvalidOperationException("Fallback is not set.");

        var result = new PreFrozenProxyRegister<TItem>();
        var molten = new MoltenRegister<TItem>(_fallback);

        // 也是给我玩儿上闭包了
        var set = _registrants.ToHashSet();
        foreach (var registrant in _registrants)
        {
            registrant.AcceptMoltenRegister(molten);
            registrant.RegisterDone += () =>
            {
                set.Remove(registrant);
                if (set.Count == 0) result.InnerRegister = molten.Freeze();
            };
        }

        return result;
    }
}
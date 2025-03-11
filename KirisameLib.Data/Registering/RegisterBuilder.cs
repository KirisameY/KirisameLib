using KirisameLib.Data.Registers;

namespace KirisameLib.Data.Registering;

/// <summary>
///     Builder for create an immutable <see cref="IRegister{TItem}"/>, which implemented by <see cref="FrozenRegister{TItem}"/> <br/>
///     Work with <see cref="IRegistrant{TItem}"/> and <see cref="IRegisterDoneEventSource"/>, will freeze inner <see cref="MoltenRegister{TItem}"/>
///     when the event source raised RegisterDone event and allow external access through the returned proxy object.
/// </summary>
/// <remarks> Both a fallback and an event source is necessary to build a register. </remarks>
public class RegisterBuilder<TItem>
{
    private Func<string, TItem>? _fallback;
    private IRegisterDoneEventSource? _eventSource;

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
    ///     Set register done event source, after the source raised RegisterDone event, the created register will be frozen and available.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public RegisterBuilder<TItem> WithRegisterDoneEventSource(IRegisterDoneEventSource source)
    {
        _eventSource = source;
        return this;
    }

    /// <summary>
    ///     Add a registrant for registering.
    /// </summary>
    public RegisterBuilder<TItem> AddRegistrant(IRegistrant<TItem> registrant)
    {
        _registrants.Add(registrant);
        return this;
    }

    /// <summary>
    ///     Build a register with current settings.
    /// </summary>
    /// <returns>The created register, will available after the event source raised RegisterDone event.</returns>
    /// <exception cref="InvalidOperationException"> Fallback or RegisterDoneEventSource is not set. </exception>
    public IRegister<TItem> Build()
    {
        if (_fallback is null) throw new InvalidOperationException("Fallback is not set.");
        if (_eventSource is null) throw new InvalidOperationException("RegisterDoneEventSource is not set.");

        var result = new PreFrozenProxyRegister<TItem>();
        var molten = new MoltenRegister<TItem>(_fallback);

        _eventSource.RegisterDone += () => result.InnerRegister = molten.Freeze();
        foreach (var registrant in _registrants)
        {
            registrant.AcceptTarget(molten);
        }

        return result;
    }
}
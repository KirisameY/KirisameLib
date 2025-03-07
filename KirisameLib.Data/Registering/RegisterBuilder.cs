using KirisameLib.Data.Registers;

namespace KirisameLib.Data.Registering;

public class RegisterBuilder<T>
{
    private Func<string, T>? _fallback;

    private readonly HashSet<IRegistrant<T>> _registrants = [];

    public RegisterBuilder<T> WithFallback(T fallback) => WithFallback(_ => fallback);

    public RegisterBuilder<T> WithFallback(Func<string, T> fallback)
    {
        _fallback = fallback;
        return this;
    }

    public RegisterBuilder<T> WithRegistrant(IRegistrant<T> registrant)
    {
        _registrants.Add(registrant);
        return this;
    }

    public IRegister<T> Build()
    {
        if (_fallback is null) throw new InvalidOperationException("Fallback is not set.");

        var result = new PreFrozenProxyRegister<T>();
        var molten = new MoltenRegister<T>(_fallback);

        // 也是给我玩儿上闭包了
        var set = _registrants.ToHashSet();
        foreach (var registrant in _registrants)
        {
            registrant.AcceptMoltenRegister(molten);
            registrant.RegisterDone += () =>
            {
                set.Remove(registrant);
                if (set.Count == 0) result.InnerRegister = molten.Frozen();
            };
        }

        return result;
    }
}
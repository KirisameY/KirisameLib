using System.Diagnostics.CodeAnalysis;

using KirisameLib.Data.Registers;

namespace KirisameLib.Data.Registering;

internal class PreFrozenProxyRegister<T> : IRegister<T>
{
    [field: AllowNull, MaybeNull]
    internal IRegister<T> InnerRegister
    {
        get => field ?? throw new RegisterNotInitializedException();
        set
        {
            if (field is not null) throw new RegisterAlreadyInitializedException();
            field = value;
        }
    }

    public T GetItem(string id) => InnerRegister.GetItem(id);

    public bool ItemRegistered(string id) => InnerRegister.ItemRegistered(id);
}

public class RegisterNotInitializedException() : InvalidOperationException("Try to visit register before registration done");

public class RegisterAlreadyInitializedException() : InvalidOperationException("Register already initialized");
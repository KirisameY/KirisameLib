using System.Collections;
using System.Diagnostics.CodeAnalysis;

using KirisameLib.Data.Registers;

namespace KirisameLib.Data.Registering;

/// <summary>
///     Internal class, used for <see cref="RegisterBuilder{TItem}"/>.
/// </summary>
internal class PreFrozenProxyRegister<TItem> : IEnumerableRegister<TItem>
{
    [field: AllowNull, MaybeNull]
    internal IEnumerableRegister<TItem> InnerRegister
    {
        get => field ?? throw new RegisterNotInitializedException();
        set
        {
            if (field is not null) throw new RegisterAlreadyInitializedException();
            field = value;
        }
    }


    public TItem this[string key] => InnerRegister[key];
    public IEnumerable<string> Keys => InnerRegister.Keys;
    public IEnumerable<TItem> Values => InnerRegister.Values;
    public int Count => InnerRegister.Count;

    public TItem GetItem(string id) => InnerRegister.GetItem(id);
    public bool ItemRegistered(string id) => InnerRegister.ItemRegistered(id);
    public bool TryGetValue(string key, [MaybeNullWhen(false)] out TItem value) => InnerRegister.TryGetValue(key, out value);

    public IEnumerator<KeyValuePair<string, TItem>> GetEnumerator() => InnerRegister.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)InnerRegister).GetEnumerator();
}

public class RegisterNotInitializedException() : InvalidOperationException("Try to visit register before registration done");

public class RegisterAlreadyInitializedException() : InvalidOperationException("Register already initialized");
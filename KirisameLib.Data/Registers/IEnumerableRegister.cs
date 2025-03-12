namespace KirisameLib.Data.Registers;

// ReSharper disable once PossibleInterfaceMemberAmbiguity
/// <summary>
///     Readonly register interface that can be iterated as a dictionary.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public interface IEnumerableRegister<TItem> : IRegister<TItem>,IReadOnlyDictionary<string,TItem>
{
    bool IReadOnlyDictionary<string, TItem>.ContainsKey(string key) => ItemRegistered(key);
}
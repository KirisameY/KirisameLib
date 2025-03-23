namespace KirisameLib.Data.Registers;

/// <summary>
///     Readonly register interface.
/// </summary>
/// <typeparam name="TKey">Type of item id</typeparam>
/// <typeparam name="TItem">Type of item to be registered</typeparam>

public interface IRegister<in TKey, out TItem> where TKey : notnull
{
    //todo: 等待拓展类型
    TItem this[TKey id] { get; }

    /// <summary>
    ///     Get registered item by id.
    /// </summary>
    /// <remarks>
    ///     Note that when implement this method, it should not throw exception when given id is not registered.
    ///     Instead, consider a default value or fallback delegate.
    /// </remarks>
    TItem GetItem(TKey id);

    /// <summary>
    ///     Check if given id is registered.
    /// </summary>
    bool ItemRegistered(TKey id);
}
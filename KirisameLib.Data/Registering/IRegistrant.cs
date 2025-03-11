using KirisameLib.Data.Registers;

namespace KirisameLib.Data.Registering;

/// <summary>
///     Interface for registering item into a <see cref="IRegTarget{TItem}"/>.
/// </summary>
/// <seealso cref="RegisterBuilder{TItem}"/>
public interface IRegistrant<TItem>
{
    void AcceptTarget(IRegTarget<TItem> moltenRegister);
}
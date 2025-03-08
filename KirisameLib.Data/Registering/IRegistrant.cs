using KirisameLib.Data.Registers;

namespace KirisameLib.Data.Registering;

/// <summary>
///     Interface for registering.
/// </summary>
/// <seealso cref="RegisterBuilder{TItem}"/>
public interface IRegistrant<TItem>
{
    void AcceptMoltenRegister(MoltenRegister<TItem> moltenRegister);
}
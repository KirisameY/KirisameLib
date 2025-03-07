using KirisameLib.Data.Registers;

namespace KirisameLib.Data.Registering;

public interface IRegistrant<TItem>
{
    void AcceptMoltenRegister(MoltenRegister<TItem> moltenRegister);
    event Action RegisterDone;
}
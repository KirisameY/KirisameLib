using KirisameLib.Data.Register;
using KirisameLib.Data.Registers;

namespace KirisameLib.Data.Registering;

public interface IRegistrant<T>
{
    void AcceptMoltenRegister(MoltenRegister<T> moltenRegister);
    event Action RegisterDone;
}
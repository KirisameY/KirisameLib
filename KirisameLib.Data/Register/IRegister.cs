namespace KirisameLib.Data.Register;

public interface IRegister<T>
{
    T GetItem(string id);
    bool ItemRegistered(string id);
}
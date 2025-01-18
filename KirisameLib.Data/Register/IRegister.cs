namespace KirisameLib.Data.Register;

public interface IRegister<T>
{
    T this[string id] => GetItem(id);
    T GetItem(string id);
    bool ItemRegistered(string id);
}
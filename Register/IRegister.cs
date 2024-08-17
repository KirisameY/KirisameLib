namespace KirisameLib.Register;

public interface IRegister<T>
{
    string Name { get; }
    bool RegisterItem(string id, T item);
    T GetItem(string id);
}
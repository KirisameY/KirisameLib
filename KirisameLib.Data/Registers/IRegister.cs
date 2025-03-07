namespace KirisameLib.Data.Registers;

public interface IRegister<T>
{
    T this[string id] => GetItem(id); //todo: 等待拓展类型
    T GetItem(string id);
    bool ItemRegistered(string id);
}
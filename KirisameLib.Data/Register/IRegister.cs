namespace KirisameLib.Data.Register;

public interface IRegister<T>
{
    T GetItem(string id);
}
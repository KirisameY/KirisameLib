namespace KirisameLib.Core.Register;

public interface IRegister<T>
{
    T GetItem(string id);
}
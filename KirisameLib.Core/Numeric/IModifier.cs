namespace KirisameLib.Numeric;

public interface IModifier<T>
{
    IModifierType<T> Type { get; }
    double Value { get; set; }
    void Modify(ref T value);
}
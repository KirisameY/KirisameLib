namespace KirisameLib.Numeric;

public interface IModifierType<T> : IComparable<IModifierType<T>>
{
    delegate void ModifyFunc(double mod, ref T value);

    ModifyFunc Modify { get; }
    int Order { get; }
    IModifier<T> NewInstance(double value);
}
namespace KirisameLib.Numeric;

public class CommonModifier<T>(IModifierType<T> type, double value) : IModifier<T>
{
    public IModifierType<T> Type { get; } = type;
    IModifierType<T> IModifier<T>.Type => Type;
    public double Value { get; set; } = value;


    public void Modify(ref T value)
    {
        Type.Modify(Value, ref value);
    }
}
namespace KirisameLib.Numeric;

public class CommonModifierType<T>(int order, IModifierType<T>.ModifyFunc modify) : IModifierType<T>
{
    public int Order { get; } = order;

    public int CompareTo(IModifierType<T>? other) => Order.CompareTo(other?.Order);

    public IModifier<T> NewInstance(double value) => new CommonModifier<T>(this, value);

    public IModifierType<T>.ModifyFunc Modify { get; } = modify;
}
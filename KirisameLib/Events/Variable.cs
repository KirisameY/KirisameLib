namespace KirisameLib.Events;

public class Variable<T>(T value)
{
    private T Value { get; set; } = value;
    public static implicit operator T(Variable<T> v) => v.Value;
}
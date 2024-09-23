namespace KirisameLib.Core.Events;

public class Variable<T>(T value)
{
    public T Value { get; set; } = value;
    public static implicit operator T(Variable<T> v) => v.Value;
}
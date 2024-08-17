using System.Numerics;

namespace KirisameLib.Numeric;

public class RangedNumeric<T>(T minValue = default, T maxValue = default, T value = default) : NumericBase<T>
    where T : struct, INumber<T>
{
    public override T BaseValue { get; set; } = value;
    public override T FinalValue => MathSpark.Clamp(BaseValue, MinValue, MaxValue);

    public T MinValue { get; set; } = minValue;
    public T MaxValue { get; set; } = maxValue;
}
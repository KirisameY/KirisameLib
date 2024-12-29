using System.Numerics;

namespace KirisameLib.Numeric;

public abstract class NumericBase<T>
    where T : struct, INumber<T>
{
    public abstract T BaseValue { get; set; }
    // ReSharper disable once MemberCanBeProtected.Global
    public abstract T FinalValue { get; }


    public static implicit operator T(NumericBase<T> numeric) => numeric.FinalValue;
}
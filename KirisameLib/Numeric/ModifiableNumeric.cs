using System.Numerics;

namespace KirisameLib.Numeric;

public class ModifiableNumeric<T>(T baseValue) : NumericBase<T>
    where T : struct, INumber<T>
{
    public override T BaseValue { get; set; } = baseValue;
    public override T FinalValue => CalculateFinalValue();

    private T CalculateFinalValue()
    {
        var value = BaseValue;
        foreach (var modifierList in ModifierLists.Values)
        {
            modifierList.ForEach(modifier => modifier.Modify(ref value));
        }

        return value;
    }

    private SortedList<IModifierType<T>, List<IModifier<T>>> ModifierLists { get; } = [];

    public void AddModifier(IModifier<T> modifier)
    {
        if (!ModifierLists.TryGetValue(modifier.Type, out var list))
        {
            list = [];
            ModifierLists.Add(modifier.Type, list);
        }

        list.Add(modifier);
    }

    public void RemoveModifier(IModifier<T> modifier)
    {
        if (!ModifierLists.TryGetValue(modifier.Type, out var list)) return;
        list.Remove(modifier);
    }
}
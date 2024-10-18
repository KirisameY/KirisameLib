using System.Diagnostics.CodeAnalysis;

using KirisameLib.Data.Model;

namespace KirisameLib.Data.Registration;

public abstract class Registrant<TSource>
{
    public abstract Func<bool>? TryParse(TSource source);
}

public abstract class Registrant<TSource, TModel, TTarget>(RegisterItem<TTarget> registerItem) : Registrant<TSource>
    where TModel : IModel<TSource, TTarget>
{
    public override Func<bool>? TryParse(TSource source)
    {
        var model = TModel.FromSource(source);
        return model is null ? null
            : () => registerItem(model.Id, model.Convert());
    }
}
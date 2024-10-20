using KirisameLib.Data.Model;

namespace KirisameLib.Data.Registration;

public abstract class Registrant<TSource>
{
    public abstract (string id, Func<bool> register)[] Parse(TSource source, out ModelParseErrorInfo errorMessages);
}

public abstract class Registrant<TSource, TModel, TTarget>(RegisterItem<TTarget> registerItem) : Registrant<TSource>
    where TModel : IModel<TSource, TTarget>
{
    public override (string id, Func<bool> register)[] Parse(TSource source, out ModelParseErrorInfo errorMessages)
    {
        var models = TModel.FromSource(source, out errorMessages);
        return models.Select(GetRegister).ToArray();

        (string id, Func<bool> register) GetRegister(IModel<TTarget> model)
        {
            return (model.Id, () => registerItem(model.Id, model.Convert()));
        }
    }
}
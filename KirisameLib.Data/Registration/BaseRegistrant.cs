using KirisameLib.Data.Model;

namespace KirisameLib.Data.Registration;

public abstract class BaseRegistrant<TSource, TModel, TTarget>(RegisterItem<TTarget> registerItem) : IRegistrant<TSource>
    where TModel : IModel<TSource, TTarget>
{
    public virtual (string id, Func<bool> register)[] Parse(TSource source, out ModelParseErrorInfo errorMessages)
    {
        var models = TModel.FromSource(source, out errorMessages);
        return models.Select(GetRegister).ToArray();

        (string id, Func<bool> register) GetRegister(IModel<TTarget> model)
        {
            return (model.Id, () => registerItem(model.Id, model.Convert()));
        }
    }
}
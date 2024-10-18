using KirisameLib.Data.I18n;
using KirisameLib.Data.Model;

namespace KirisameLib.Data.Registration;

public class LocalizedRegistrant<TSource, TModel, TTarget>(LocalizedRegister<TTarget> register, Func<string> localGetter)
    : Registrant<TSource, TModel, TTarget>((id, item) => register.RegisterLocalizedItem(localGetter(), id, item))
    where TModel : IModel<TSource, TTarget> { }
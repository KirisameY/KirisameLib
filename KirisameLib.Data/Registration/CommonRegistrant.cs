using KirisameLib.Data.Model;
using KirisameLib.Data.Register;

namespace KirisameLib.Data.Registration;

public class CommonRegistrant<TSource, TModel, TTarget>(CommonRegister<TTarget> register)
    : BaseRegistrant<TSource, TModel, TTarget>(register.RegisterItem)
    where TModel : IModel<TSource, TTarget> { }
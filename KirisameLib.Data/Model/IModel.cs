namespace KirisameLib.Data.Model;

public interface IModel<out TTarget>
{
    string Id { get; }
    TTarget Convert();
}

public interface IModel<in TSource, out TTarget> : IModel<TTarget>
{
    static abstract IModel<TTarget>[] FromSource(TSource source, out ModelParseErrorInfo errorMessages);
}
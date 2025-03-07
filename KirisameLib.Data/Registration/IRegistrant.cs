using KirisameLib.Data.Model;

namespace KirisameLib.Data.Registration;

[Obsolete]
public interface IRegistrant<in TSource>
{
    (string id, Func<bool> register)[] Parse(TSource source, out ModelParseErrorInfo errorMessages);
}
using System.Diagnostics.CodeAnalysis;

using KirisameLib.Logging;
using KirisameLib.Register;

namespace KirisameLib.I18n;

// ReSharper disable once InconsistentNaming
public class I18nRegister<T>(IRegister<T> defaultRegister) : IRegister<T>
{
    //New
    public I18nRegister(string registerName, Func<string, T> defaultItemGetter) :
        this(new CommonRegister<T>(registerName, defaultItemGetter)) { }

    //Member
    public string Name => DefaultRegister.Name;
    private Dictionary<string, Dictionary<string, T>> LocalRegisterDict { get; } = [];
    private IRegister<T> DefaultRegister { get; } = defaultRegister;

    //Register methods
    public bool RegisterLocalizedItem(string local, string id, T item)
    {
        const string loggingProcessName = "LocalRegistering";

        LocalRegisterDict.TryGetValue(local, out var regDict);
        if (regDict is null)
        {
            regDict = [];
            LocalRegisterDict.Add(local, regDict);
            Logger.Log(LogLevel.Debug, loggingProcessName, $"Local dict {local} not exists, created");
        }

        var succeed = regDict.TryAdd(id, item);
        if (succeed)
            Logger.Log(LogLevel.Debug, loggingProcessName, $"Item ID '{id}' registered successfully");
        else
            Logger.Log(LogLevel.Warning, loggingProcessName, $"The item ID '{id}' trying to be registered has already been registered");
        return succeed;
    }

    public bool RegisterItem(string id, T item) => DefaultRegister.RegisterItem(id, item);

    public T GetItem(string id)
    {
        const string loggingProcessName = "GettingItem";

        if (GetItemInLocal(LocalSettings.Local, id, out var item)) return item;
        Logger.Log(LogLevel.Debug, loggingProcessName, $"Item id '{id}' not exist in current local, try to get in default local");

        if (GetItemInLocal(LocalSettings.DefaultLocal, id, out item)) return item;
        Logger.Log(LogLevel.Debug, loggingProcessName, $"Item id '{id}' not exist in default local, try to get in default register");

        return DefaultRegister.GetItem(id);
    }

    private bool GetItemInLocal(string local, string id, [NotNullWhen(true)] out T? item)
    {
        item = default(T);
        if (!LocalRegisterDict.TryGetValue(local, out var regDict))
            return false;
        return regDict.TryGetValue(id, out item);
    }

    //Logging
    private Logger Logger { get; } = LogManager.GetLogger($"Register.{defaultRegister.Name}");
}
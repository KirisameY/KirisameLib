using System.Diagnostics.CodeAnalysis;

using KirisameLib.Core.Events;
using KirisameLib.Core.Logging;
using KirisameLib.Core.Register;

namespace KirisameLib.Core.I18n;

public abstract class LocalizedRegister
{
    private static string? _local;
    public static string Local
    {
        get => _local ?? System.Globalization.CultureInfo.InstalledUICulture.TwoLetterISOLanguageName;
        set
        {
            value = value.ToLower();
            var prev = Local;
            _local = value;
            StaticLogger.Log(LogLevel.Info, "Setting", $"Local changed to '{value}' from '{prev}'");
            EventBus.Publish(new LocalChangedEvent(prev, value));
        }
    }


    //Logging
    private static Logger StaticLogger { get; } = LogManager.GetLogger("Register.Localized");
}

public class LocalizedRegister<T> : LocalizedRegister, IRegister<T>
{
    #region Constructors

    private LocalizedRegister(string registerName, Func<LocalizedRegister<T>, string, T> defaultGetter)
    {
        Name = registerName;
        DefaultGetter = defaultGetter;
        Logger = LogManager.GetLogger($"Register.Localized.{registerName}");
    }

    public LocalizedRegister(string registerName, Func<string, T> defaultGetter) :
        this(registerName, (_, id) => defaultGetter(id)) { }

    public LocalizedRegister(string registerName, IRegister<T> defaultRegister) :
        this(registerName, (_, id) => defaultRegister.GetItem(id)) { }

    public LocalizedRegister(string registerName, string defaultLocal, Func<string, T> defaultGetter) :
        this(registerName, (@this, id) =>
                 @this.GetItemInLocal(defaultLocal, id, out var item)
                     ? item
                     : defaultGetter(id)) { }

    #endregion


    #region Members

    public string Name { get; }
    private Dictionary<string, Dictionary<string, T>> LocalRegisterDict { get; } = [];
    private Func<LocalizedRegister<T>, string, T> DefaultGetter { get; }

    #endregion


    #region Register

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

    public T GetItem(string id)
    {
        const string loggingProcessName = "GettingItem";

        if (GetItemInLocal(Local, id, out var item)) return item;

        Logger.Log(LogLevel.Debug, loggingProcessName, $"Item id '{id}' not exist ,now get default value");
        return DefaultGetter(this, id);
    }

    private bool GetItemInLocal(string local, string id, [MaybeNullWhen(false)] out T item)
    {
        item = default(T);

        if (LocalRegisterDict.TryGetValue(local, out var regDict) &&
            regDict.TryGetValue(id, out item)) return true;

        return false;
    }

    #endregion


    #region Logging

    private Logger Logger { get; }

    #endregion
}
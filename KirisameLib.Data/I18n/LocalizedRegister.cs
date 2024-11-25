using System.Diagnostics.CodeAnalysis;

using KirisameLib.Core.Logging;
using KirisameLib.Data.Register;

namespace KirisameLib.Data.I18n;

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
            LocalChangedEvent?.Invoke(prev, value);
        }
    }


    //Event
    public delegate void LocalChangedEventHandler(string prev, string next);

    public static event LocalChangedEventHandler? LocalChangedEvent;


    //Logging
    private static Logger StaticLogger { get; } = LogManager.GetLogger("Register.Localized");
}

public class LocalizedRegister<T> : LocalizedRegister, IRegister<T>
{
    #region Constructors

    private LocalizedRegister(string registerName,
                              Func<LocalizedRegister<T>, string, T> defaultGetter,
                              Func<LocalizedRegister<T>, string, bool> defaultFinder)
    {
        Name = registerName;
        DefaultGetter = defaultGetter;
        DefaultFinder = defaultFinder;
        Logger = LogManager.GetLogger($"Register.Localized.{registerName}");
    }

    public LocalizedRegister(string registerName, Func<string, T> defaultGetter, Func<string, bool>? defaultFinder = null) :
        this(registerName,
             (_, id) => defaultGetter(id),
             (_, id) => defaultFinder?.Invoke(id) ?? false) { }

    public LocalizedRegister(string registerName, IRegister<T> defaultRegister) :
        this(registerName,
             (_, id) => defaultRegister.GetItem(id),
             (_, id) => defaultRegister.ItemRegistered(id)) { }

    public LocalizedRegister(string registerName, string defaultLocal, Func<string, T> defaultGetter,
                             Func<string, bool>? defaultFinder = null) :
        this(registerName,
             (@this, id) =>
                 @this.GetItemInLocal(defaultLocal, id, out var item)
                     ? item
                     : defaultGetter(id),
             (@this, id) =>
                 @this.ItemRegisteredInLocal(defaultLocal, id) || (defaultFinder?.Invoke(id) ?? false)
            ) { }

    #endregion


    #region Members

    public string Name { get; }
    private Dictionary<string, Dictionary<string, T>> LocalRegisterDict { get; } = [];
    private Func<LocalizedRegister<T>, string, T> DefaultGetter { get; }
    private Func<LocalizedRegister<T>, string, bool> DefaultFinder { get; }

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

    public bool ItemRegistered(string id)
    {
        return ItemRegisteredInLocal(Local, id) || DefaultFinder(this, id);
    }

    private bool GetItemInLocal(string local, string id, [MaybeNullWhen(false)] out T item)
    {
        item = default(T);

        if (LocalRegisterDict.TryGetValue(local, out var regDict) &&
            regDict.TryGetValue(id, out item)) return true;

        return false;
    }

    public bool ItemRegisteredInLocal(string local, string id)
    {
        return LocalRegisterDict.TryGetValue(local, out var regDict) && regDict.ContainsKey(id);
    }

    #endregion


    #region Logging

    private Logger Logger { get; }

    #endregion
}
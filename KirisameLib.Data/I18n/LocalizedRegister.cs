using System.Diagnostics.CodeAnalysis;

using KirisameLib.Data.Register;
using KirisameLib.Data.Registers;

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
            LocalChangedEvent?.Invoke(prev, value);
        }
    }


    //Event
    public static event LocalChangedEventHandler? LocalChangedEvent;
}

public delegate void LocalChangedEventHandler(string prev, string current);

public class LocalizedRegister<T> : LocalizedRegister, IRegister<T>
{
    #region Constructors

    private LocalizedRegister(Func<LocalizedRegister<T>, string, T> defaultGetter,
                              Func<LocalizedRegister<T>, string, bool> defaultFinder)
    {
        DefaultGetter = defaultGetter;
        DefaultFinder = defaultFinder;
    }

    public LocalizedRegister(Func<string, T> defaultGetter, Func<string, bool>? defaultFinder = null) :
        this((_, id) => defaultGetter(id),
             (_, id) => defaultFinder?.Invoke(id) ?? false) { }

    public LocalizedRegister(IRegister<T> defaultRegister) :
        this((_, id) => defaultRegister.GetItem(id),
             (_, id) => defaultRegister.ItemRegistered(id)) { }

    public LocalizedRegister(string defaultLocal, Func<string, T> defaultGetter,
                             Func<string, bool>? defaultFinder = null) :
        this((@this, id) =>
                 @this.GetItemInLocal(defaultLocal, id, out var item)
                     ? item
                     : defaultGetter(id),
             (@this, id) =>
                 @this.ItemRegisteredInLocal(defaultLocal, id) || (defaultFinder?.Invoke(id) ?? false)
            ) { }

    #endregion


    #region Members

    private Dictionary<string, Dictionary<string, T>> LocalRegisterDict { get; } = [];
    private Func<LocalizedRegister<T>, string, T> DefaultGetter { get; }
    private Func<LocalizedRegister<T>, string, bool> DefaultFinder { get; }

    #endregion


    #region Register

    public bool RegisterLocalizedItem(string local, string id, T item)
    {
        LocalRegisterDict.TryGetValue(local, out var regDict);
        if (regDict is null)
        {
            regDict = [];
            LocalRegisterDict.Add(local, regDict);
        }

        var succeed = regDict.TryAdd(id, item);
        return succeed;
    }

    public T GetItem(string id)
    {
        if (GetItemInLocal(Local, id, out var item)) return item;

        try { return DefaultGetter(this, id); }
        catch (Exception e)
        {
            throw new GettingFallbackValueFailedException($"Failed to get default value for item: "
                                                       + $"ID: {id}, Type: {typeof(T).Name}", e);
        }
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
}
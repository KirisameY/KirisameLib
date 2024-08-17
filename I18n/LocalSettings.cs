using KirisameLib.Events;
using KirisameLib.Logging;

namespace KirisameLib.I18n;

public static class LocalSettings
{
    private static string? _local;
    public static string Local
    {
        get => _local ?? DefaultLocal;
        set
        {
            value = value.ToLower();
            var prev = Local;
            _local = value;
            Logger.Log(LogLevel.Info, "Setting", $"Local changed to '{value}' from '{prev}'");
            EventBus.Publish(new LocalChangedEvent(prev, value));
        }
    }

    private static string _defaultLocal = "en";
    public static string DefaultLocal
    {
        get => _defaultLocal;
        set
        {
            value = value.ToLower();
            var prev = DefaultLocal;
            _defaultLocal = value;
            Logger.Log(LogLevel.Info, "Setting", $"Default local changed to '{value}' from '{prev}'");
            EventBus.Publish(new DefaultLocalChangedEvent(prev, value));
        }
    }

    //Logging
    private static Logger Logger { get; } = LogManager.GetLogger(nameof(LocalSettings));
}
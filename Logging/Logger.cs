namespace KirisameLib.Logging;

public class Logger
{
    internal Logger(string source, Action<Log> logAction)
    {
        Source = source;
        LogAction = logAction;
    }

    public string Source { get; init; }
    private Action<Log> LogAction { get; init; }

    public void Log(LogLevel level, string process, string message) => LogAction(new Log(level, Source, process, message));
}
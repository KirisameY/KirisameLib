namespace KirisameLib.Logging;

public record Log(LogLevel Level, string Source, string Process, string Message)
{
    private DateTime Time { get; init; } = DateTime.Now;

    public override string ToString() =>
        $"{Time:yyyy-MM-dd HH:mm:ss.fff} |{Level}| [{Source}] [{Process}]: {Message}";
}

public enum LogLevel
{
    Debug,
    Info,
    Warning,
    Error,
    Fatal,
    Disable,
}
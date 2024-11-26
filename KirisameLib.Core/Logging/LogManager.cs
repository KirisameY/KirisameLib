﻿using System.Collections.Concurrent;

namespace KirisameLib.Core.Logging;

//todo: 讲真Log不宜直接用，该throw就throw该Try和out就Try然后out，不要干涉用户的日志怎么整（回头直接把log搬到新项目里
public static partial class LogManager
{
    private static ConcurrentQueue<Log> LogQueue { get; } = [];
    private static LogWriter? Writer { get; set; }

    private static Action<string>? RuntimePrinter { get; set; }

    private static bool Initialized { get; set; } = false;

    public static void Initialize(LogLevel minWriteLevel, LogLevel minPrintLevel, Action<string> runtimePrinter, string logDirPath,
                                  string logFileName, int maxLogFileCount)
    {
        if (Initialized)
        {
            Log(new(LogLevel.Warning, nameof(LogManager), "Initializing", "Request for duplicate initialization of Logger"));
        }

        MinWriteLevel = minWriteLevel;
        MinPrintLevel = minPrintLevel;

        Writer = new(LogQueue, logDirPath, logFileName, maxLogFileCount);
        RuntimePrinter = runtimePrinter;
        Initialized = true;
    }

    public static void Dispose()
    {
        Writer?.Close();
    }

    //Logging
    private static Dictionary<string, Logger> Loggers { get; } = [];

    public static Logger GetLogger(string source)
    {
        if (!Loggers.TryGetValue(source, out Logger? logger))
        {
            logger = new Logger(source, Log);
            Loggers.Add(source, logger);
        }

        return logger;
    }

    private static void Log(Log log)
    {
        if (log.Level >= MinPrintLevel)
            RuntimePrinter?.Invoke(log.ToString());
        if (log.Level >= MinWriteLevel)
            LogQueue.Enqueue(log);
    }

    public static LogLevel MinWriteLevel { get; private set; }
    public static LogLevel MinPrintLevel { get; private set; }
}
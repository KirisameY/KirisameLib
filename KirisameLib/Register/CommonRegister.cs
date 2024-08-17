using KirisameLib.Logging;

namespace KirisameLib.Register;

public class CommonRegister<T>(string registerName, Func<string, T> defaultItemGetter) : IRegister<T>
{
    public string Name { get; } = registerName;
    private Dictionary<string, T> RegDict { get; } = [];
    private Func<string, T> DefaultItemGetter { get; } = defaultItemGetter;

    public bool RegisterItem(string id, T item)
    {
        const string loggingProcessName = "Registering";
        var succeed = RegDict.TryAdd(id, item);
        if (succeed)
            Logger.Log(LogLevel.Debug, loggingProcessName, $"Item ID '{id}' registered successfully");
        else
            Logger.Log(LogLevel.Warning, loggingProcessName, $"The item ID '{id}' trying to be registered has already been registered");
        return succeed;
    }

    public T GetItem(string id)
    {
        const string loggingProcessName = "GettingItem";

        if (RegDict.TryGetValue(id, out var item))
            return item;
        Logger.Log(LogLevel.Warning, loggingProcessName, $"The attempted query ID '{id}' is not registered , default value will be return");
        try
        {
            return DefaultItemGetter(id);
        }
        catch (Exception e)
        {
            var res = default(T);
            if (res is null)
            {
                Logger.Log(LogLevel.Fatal, loggingProcessName,
                           $"Exception on Getting default value: {e}, and default value of {typeof(T).Name} is null");
                throw new
                    GettingDefaultValueFailedException($"RegisterName: {Name}, DefaultGetter: {DefaultItemGetter}, Type: {typeof(T).Name}",
                                                       e);
            }

            Logger.Log(LogLevel.Error, loggingProcessName,
                       $"Exception on Getting default value: {e}, default value of {typeof(T).Name} has been returned");
            return res;
        }
    }

    //Logging
    private Logger Logger { get; } = LogManager.GetLogger($"Register.{registerName}");

    //Exception
    public class GettingDefaultValueFailedException(string message, Exception inner) : Exception(message, inner);
}
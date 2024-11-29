namespace KirisameLib.Data.Register;

public class CommonRegister<T>(Func<string, T> defaultItemGetter) : IRegister<T>
{
    private Dictionary<string, T> RegDict { get; } = [];
    private Func<string, T> DefaultItemGetter { get; } = defaultItemGetter;

    public bool RegisterItem(string id, T item)
    {
        return RegDict.TryAdd(id, item);
    }

    public T GetItem(string id)
    {
        if (RegDict.TryGetValue(id, out var item)) return item;
        
        try { return DefaultItemGetter(id); }
        catch (Exception e)
        {
            throw new GettingDefaultValueFailedException($"Failed to get default value for item: "
                                                       + $"ID: {id}, Type: {typeof(T).Name}", e);
        }
    }

    public bool ItemRegistered(string id)
    {
        return RegDict.ContainsKey(id);
    }
}
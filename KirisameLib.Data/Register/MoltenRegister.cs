namespace KirisameLib.Data.Register;

public class MoltenRegister<T>(Func<string, T> fallback) : IRegister<T>
{
    private readonly Dictionary<string, T> _regDict = new();

    public bool AddItem(string id, T item) => _regDict.TryAdd(id, item);

    public void AddOrOverwriteItem(string id, T item) => _regDict[id] = item;

    public T GetItem(string id)
    {
        if (!_regDict.TryGetValue(id, out var value))
        {
            try { value = fallback(id); }
            catch (Exception e)
            {
                throw new GettingFallbackValueFailedException(id, e);
            }
        }

        return value;
    }

    public bool ItemRegistered(string id) => _regDict.ContainsKey(id);
}
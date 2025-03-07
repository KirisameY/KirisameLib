namespace KirisameLib.Data.Registers;

public class MoltenRegister<T>(Func<string, T> fallback) : IRegister<T>
{
    private bool _frozen = false;

    private readonly Dictionary<string, T> _regDict = new();

    public bool AddItem(string id, T item)
    {
        if (_frozen) throw new FrozenException();
        return _regDict.TryAdd(id, item);
    }

    public void AddOrOverwriteItem(string id, T item)
    {
        if (_frozen) throw new FrozenException();
        _regDict[id] = item;
    }

    public T GetItem(string id)
    {
        if (_frozen) throw new FrozenException();
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

    public bool ItemRegistered(string id)
    {
        if (_frozen) throw new FrozenException();
        return _regDict.ContainsKey(id);
    }

    public FrozenRegister<T> Frozen()
    {
        if (_frozen) throw new FrozenException();
        _frozen = true;
        return new FrozenRegister<T>(_regDict, fallback);
    }

    public class FrozenException() : InvalidOperationException("Try to visit a frozen molten register.");
}
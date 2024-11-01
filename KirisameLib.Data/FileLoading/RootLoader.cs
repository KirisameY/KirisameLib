using System.Collections.ObjectModel;

using KirisameLib.Data.Registration;

namespace KirisameLib.Data.FileLoading;

public abstract class RootLoader
{
    public abstract bool EnterDirectory(string dirName);
    public abstract void LoadFile(string fileName, byte[] fileContent);

    /// <returns>true if the root directory was exited</returns>
    public abstract bool ExitDirectory();
}

public abstract class RootLoader<TSource, TRegistrant> : RootLoader
    where TRegistrant : IRegistrant<TSource>
{
    private Dictionary<string, TRegistrant> PathMap { get; } = new();
    private ReadOnlyDictionary<string, TRegistrant>? _pathMapView;
    protected ReadOnlyDictionary<string, TRegistrant> PathMapView => _pathMapView ??= PathMap.AsReadOnly();
    public bool AddRegistrant(string path, TRegistrant registrant) => PathMap.TryAdd(path, registrant);
}
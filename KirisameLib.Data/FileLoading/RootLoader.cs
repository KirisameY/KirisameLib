using System.Collections.ObjectModel;

using KirisameLib.Data.Registration;

namespace KirisameLib.Data.FileLoading;

public abstract class RootLoader<TSource>
{
    private Dictionary<string, Registrant<TSource>> PathMap { get; } = new();
    private ReadOnlyDictionary<string, Registrant<TSource>>? _pathMapView;
    protected ReadOnlyDictionary<string, Registrant<TSource>> PathMapView => _pathMapView ??= PathMap.AsReadOnly();


    public abstract bool EnterDirectory(string dirName);
    public abstract void LoadFile(string fileName, byte[] fileContent);
    
    /// <returns>true if the root directory was exited</returns>
    public abstract bool ExitDirectory();
}
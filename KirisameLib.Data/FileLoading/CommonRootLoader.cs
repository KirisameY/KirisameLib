using KirisameLib.Core.Extensions;
using KirisameLib.Data.Register;
using KirisameLib.Data.Registration;

namespace KirisameLib.Data.FileLoading;

public abstract class CommonRootLoader<TSource> : RootLoader<TSource>
{
    private LinkedList<string> PathLink { get; } = [];
    private Stack<RegisterInfo> RegisterStack { get; } = [];


    public sealed override bool EnterDirectory(string dirName)
    {
        PathLink.AddLast(dirName);
        var path = PathLink.Join('/');
        if (PathMapView.TryGetValue(path, out var registrant))
        {
            RegisterStack.Push(new RegisterInfo(path, registrant, []));
        }
        return RegisterStack.Count > 0;
    }

    public sealed override void LoadFile(string fileName, byte[] fileContent)
    {
        if (!RegisterStack.TryPeek(out var info)) return;
        HandleFile(info.SourceDict, fileName, fileContent);
    }

    public sealed override bool ExitDirectory()
    {
        if (PathLink.Count == 0)
        {
            EndUp();
            return true;
        }
        var path = PathLink.Join('/');
        PathLink.RemoveLast();
        if (!RegisterStack.TryPeek(out var info) || info.Path != path) return false;
        RegisterStack.Pop();
        RegisterDirectory(info);
        return false;
    }


    protected abstract void HandleFile(Dictionary<string, TSource> sourceDict, string fileName, byte[] fileContent);
    protected abstract void RegisterDirectory(RegisterInfo info);
    protected abstract void EndUp();


    protected readonly record struct RegisterInfo(string Path, Registrant<TSource> Registrant, Dictionary<string, TSource> SourceDict);
}
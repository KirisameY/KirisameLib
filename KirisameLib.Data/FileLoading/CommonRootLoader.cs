using KirisameLib.Data.Registration;
using KirisameLib.Extensions;

namespace KirisameLib.Data.FileLoading;

public abstract class CommonRootLoader<TSource, TRegistrant> : RootLoader<TSource, TRegistrant>
    where TRegistrant : IRegistrant<TSource>
{
    private LinkedList<string> PathLink { get; } = [];
    private Stack<RegisterInfo> RegisterStack { get; } = [];
    private List<Task> RegisteringTasks { get; } = [];
    private bool Exited { get; set; } = false;

    protected string CurrentPath => RegisterStack.TryPeek(out var info) ? info.Path : "";


    public sealed override bool EnterDirectory(string dirName)
    {
        RootLoaderExitedException.ThrowIf(Exited);

        PathLink.AddLast(dirName);
        var path = PathLink.Join('/');
        if (PathMapView.TryGetValue(path, out var registrant))
        {
            RegisterStack.Push(new RegisterInfo(path, registrant, [], []));
        }
        else if (RegisterStack.TryPeek(out var info))
        {
            info.SubPathLink.AddLast(dirName);
        }

        return RegisterStack.Count > 0;
    }

    public sealed override void LoadFile(string fileName, byte[] fileContent)
    {
        RootLoaderExitedException.ThrowIf(Exited);

        if (!RegisterStack.TryPeek(out var info)) return;
        HandleFile(info.SourceDict, info.SubPathLink.Append(fileName).Join('/'), fileContent);
    }

    public sealed override bool ExitDirectory()
    {
        RootLoaderExitedException.ThrowIf(Exited);

        if (PathLink.Count == 0)
        {
            Task.WhenAll(RegisteringTasks).Wait();
            EndUp();
            Exited = true;
            return true;
        }

        //else: still in root directory
        var path = PathLink.Join('/');
        PathLink.RemoveLast();

        if (!RegisterStack.TryPeek(out var info)) return false;
        if (info.SubPathLink.Count > 0) info.SubPathLink.RemoveLast();
        if (info.Path != path) return false;

        var task = RegisterDirectory(info.Registrant, info.SourceDict);
        RegisteringTasks.Add(task);
        RegisterStack.Pop();
        return false;
    }


    protected abstract void HandleFile(Dictionary<string, TSource> sourceDict, string fileSubPath, byte[] fileContent);
    protected abstract Task RegisterDirectory(TRegistrant registrant, Dictionary<string, TSource> sourceDict);
    protected abstract void EndUp();


    private readonly record struct RegisterInfo(
        string Path, TRegistrant Registrant,
        Dictionary<string, TSource> SourceDict,
        LinkedList<string> SubPathLink
    );
}
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using KirisameLib.Core.Extensions;
using KirisameLib.Data.Registration;

namespace KirisameLib.Data.FileLoading;

public abstract class CommonRootLoader<TSource, TRegistrant> : RootLoader<TSource, TRegistrant>
    where TRegistrant : Registrant<TSource>
{
    private LinkedList<string> PathLink { get; } = [];
    private Stack<RegisterInfo> RegisterStack { get; } = [];
    private bool Exited { get; set; } = false;


    public sealed override bool EnterDirectory(string dirName)
    {
        RootLoaderExitedException.ThrowIf(Exited);

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
        RootLoaderExitedException.ThrowIf(Exited);

        if (!RegisterStack.TryPeek(out var info)) return;
        HandleFile(info.SourceDict, fileName, fileContent);
    }

    public sealed override bool ExitDirectory()
    {
        RootLoaderExitedException.ThrowIf(Exited);

        if (PathLink.Count == 0)
        {
            EndUp();
            Exited = true;
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


    protected readonly record struct RegisterInfo(string Path, TRegistrant Registrant, Dictionary<string, TSource> SourceDict);
}

public class RootLoaderExitedException : Exception
{
    internal RootLoaderExitedException() { }

    [StackTraceHidden]
    internal static void ThrowIf([DoesNotReturnIf(true)] bool condition)
    {
        if (condition) throw new RootLoaderExitedException();
    }
}
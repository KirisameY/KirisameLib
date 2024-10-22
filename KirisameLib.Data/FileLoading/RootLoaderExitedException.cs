using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace KirisameLib.Data.FileLoading;

public class RootLoaderExitedException : Exception
{
    [StackTraceHidden]
    public static void ThrowIf([DoesNotReturnIf(true)] bool condition)
    {
        if (condition) throw new RootLoaderExitedException();
    }
}
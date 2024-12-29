using System.Runtime.CompilerServices;

namespace KirisameLib.Asynchronous;

public static class AsyncOrrery
{
    public static ConfiguredTaskAwaitable SwitchContext() => Task.Run(static () => { }, CancellationToken.None).ConfigureAwait(false);
}
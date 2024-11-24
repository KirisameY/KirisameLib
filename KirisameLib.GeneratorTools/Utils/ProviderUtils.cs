using Microsoft.CodeAnalysis;

namespace KirisameLib.GeneratorTools.Utils;

public static class ProviderUtils
{
    public static IncrementalValuesProvider<T> WhereNotNull<T>(this IncrementalValuesProvider<T?> provider) =>
        provider.Where(x => x is not null).Select((x, _) => x!);
}
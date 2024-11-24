using Microsoft.CodeAnalysis;

namespace KirisameLib.GeneratorTools.Utils;

public static class SymbolUtils
{
    public static bool IsDerivedFrom(this INamedTypeSymbol? type, INamedTypeSymbol baseType)
    {
        while (type != null)
        {
            if (SymbolEqualityComparer.Default.Equals(type, baseType)) return true;
            type = type.BaseType;
        }
        return false;
    }

    public static bool IsDerivedFrom(this INamedTypeSymbol? type, string baseType)
    {
        while (type != null)
        {
            if (type.ToDisplayString() == baseType) return true;
            type = type.BaseType;
        }
        return false;
    }
}
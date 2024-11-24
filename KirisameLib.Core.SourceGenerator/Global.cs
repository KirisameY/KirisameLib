using System.CodeDom.Compiler;

namespace KirisameLib.Core.SourceGenerator;

public class Global
{
    public const string GeneratorName = "KirisameLib.Core.SourceGenerator";
    public const string Version = "0.1.0.0";
    public const string GeneratedCodeAttribute = $"[global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"{GeneratorName}\", \"{Version}\")]";
}
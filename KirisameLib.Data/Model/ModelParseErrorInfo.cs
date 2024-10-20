namespace KirisameLib.Data.Model;

public record struct ModelParseErrorInfo(int ErrorCount, string[] Messages);
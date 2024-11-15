namespace KirisameLib.Data.Model;

public record struct ModelParseErrorInfo(int ErrorCount, string[] Messages)
{
    public static ModelParseErrorInfo Empty => new(0, []);
}
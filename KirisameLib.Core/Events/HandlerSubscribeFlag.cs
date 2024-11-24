namespace KirisameLib.Core.Events;

[Flags]
public enum HandlerSubscribeFlag
{
    None = 0,
    OnlyOnce = 1 << 0,
    Counted = 1 << 1,
    AllowMultiple = 1 << 2,
}
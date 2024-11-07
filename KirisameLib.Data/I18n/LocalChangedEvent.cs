using KirisameLib.Core.Events;

namespace KirisameLib.Data.I18n;

public record LocalSettingEvent : BaseEvent;

public record LocalChangedEvent(string PreviousLocal, string NewLocal) : BaseEvent;
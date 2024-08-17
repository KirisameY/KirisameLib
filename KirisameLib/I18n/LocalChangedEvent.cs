using KirisameLib.Events;

namespace KirisameLib.I18n;

public record LocalSettingEvent : BaseEvent;

public record LocalChangedEvent(string PreviousLocal, string NewLocal) : BaseEvent;

public record DefaultLocalChangedEvent(string PreviousLocal, string NewLocal) : BaseEvent;
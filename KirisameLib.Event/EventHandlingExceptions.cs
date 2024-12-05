using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace KirisameLib.Event;

public class EventHandlingException(IEnumerable<Exception> innerExceptions, BaseEvent @event) : AggregateException(innerExceptions)
{
    public BaseEvent FromEvent => @event;
    public override string ToString() => $"FromEvent:{FromEvent}, {base.ToString()}";
}

public class QueueEventHandlingException(IEnumerable<EventHandlingException> innerExceptions) : AggregateException(innerExceptions)
{
    [field: AllowNull, MaybeNull]
    public ReadOnlyCollection<EventHandlingException> EventHandlingExceptions => field ??= new(innerExceptions.ToList());
}
namespace KirisameLib.Core.Events;

public sealed class ImmediateEventBus : EventBus
{
    private bool _handlingEvent = false;

    protected override void EventReceived()
    {
        if (_handlingEvent) return;

        //handle event queue
        _handlingEvent = true;
        List<EventHandlingException> exceptions = [];

        while (NotifyQueue.TryDequeue(out var notifyAction))
        {
            try
            {
                notifyAction.Invoke();
            }
            catch (EventHandlingException e)
            {
                exceptions.Add(e);
            }
        }
        _handlingEvent = false;

        if (exceptions.Count > 0) throw new QueueEventHandlingException(exceptions);
    }
}
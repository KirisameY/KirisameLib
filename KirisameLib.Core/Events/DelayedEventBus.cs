﻿namespace KirisameLib.Core.Events;

public sealed class DelayedEventBus : EventBus
{
    private bool _handlingEvent = false;
    private readonly Queue<Action> _subNotifyQueue = [];

    protected override void NotifyEnqueue(Action action)
    {
        if (_handlingEvent) _subNotifyQueue.Enqueue(action);
        else NotifyQueue.Enqueue(action);
    }

    protected override void EventReceived() { }

    public void HandleEvent()
    {
        if (_handlingEvent) return;

        //handle main event queue
        _handlingEvent = true;
        List<EventHandlingException> exceptions = [];

        while (NotifyQueue.TryDequeue(out var notifyAction))
        {
            InvokeAndCatch(notifyAction);
            //handle sub event queue after every main event to ensure event order
            while (_subNotifyQueue.TryDequeue(out var subNotifyAction))
            {
                InvokeAndCatch(subNotifyAction);
            }
        }
        _handlingEvent = false;

        if (exceptions.Count > 0) throw new QueueEventHandlingException(exceptions);
        return;

        void InvokeAndCatch(Action notify)
        {
            try
            {
                notify.Invoke();
            }
            catch (EventHandlingException e)
            {
                exceptions.Add(e);
            }
        }
    }
}
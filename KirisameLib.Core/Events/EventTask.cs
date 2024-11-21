﻿using System.Runtime.CompilerServices;

namespace KirisameLib.Core.Events;

public class EventTask<TEvent> where TEvent : BaseEvent
{
    internal EventTask(TEvent @event, Action<EventTask<TEvent>> readyAction)
    {
        Event = @event;
        _readyAction = readyAction;
    }

    private readonly Action<EventTask<TEvent>> _readyAction;
    private Action? _continueAction;

    public bool IsReady { get; private set; } = false;
    public bool IsCompleted { get; private set; } = false;

    public TEvent Event { get; private init; }


    internal void Complete()
    {
        IsCompleted = true;
        _continueAction?.Invoke();
    }

    public EventAwaiter<TEvent> GetAwaiter() => new(this);

    public EventTask<TEvent> ContinueWith(Action continuation)
    {
        if (IsCompleted) continuation.Invoke();
        else _continueAction += continuation;
        return this;
    }

    public EventTask<TEvent> ContinueWith(Action<TEvent> continuation)
    {
        var action = () => continuation.Invoke(Event);
        if (IsCompleted) action.Invoke();
        else _continueAction += action;
        return this;
    }

    public void Ready()
    {
        if (IsReady) return;
        IsReady = true;
        _readyAction.Invoke(this);
    }
}

public readonly struct EventAwaiter<TEvent> : INotifyCompletion where TEvent : BaseEvent
{
    internal EventAwaiter(EventTask<TEvent> task)
    {
        _task = task;
    }

    private readonly EventTask<TEvent> _task;

    public TEvent GetResult() => IsCompleted ? _task.Event : throw new InvalidOperationException("Event is not completed.");

    public bool IsCompleted => _task.IsCompleted;

    public void OnCompleted(Action continuation) => _task.ContinueWith(continuation).Ready();
}
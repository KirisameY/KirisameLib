using System.Runtime.CompilerServices;

namespace KirisameLib.Core.Events;

public class EventTask<TEvent> where TEvent : BaseEvent
{
    internal EventTask(TEvent @event)
    {
        Event = @event;
    }

    private Action? _continueAction;

    internal void Complete()
    {
        IsCompleted = true;
        _continueAction?.Invoke();
    }

    public bool IsCompleted { get; private set; } = false;

    public TEvent Event { get; private init; }

    public EventAwaiter<TEvent> GetAwaiter() => new(this);

    public void ContinueWith(Action continuation) => _continueAction += continuation;
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

    public void OnCompleted(Action continuation) => _task.ContinueWith(continuation);
}
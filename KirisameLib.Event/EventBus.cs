namespace KirisameLib.Event;

public abstract class EventBus
{
    private readonly Dictionary<Type, HandlerInfos> _handlersDict = new();
    protected readonly Queue<Action> NotifyQueue = [];

    /// <returns>
    ///     Callback to remove registered handler, useful when unregistering an anonymous function
    /// </returns>
    public Action Subscribe<TEvent>(Action<TEvent> handler, HandlerSubscribeFlag flags = HandlerSubscribeFlag.None)
        where TEvent : BaseEvent
    {
        if (!_handlersDict.TryGetValue(typeof(TEvent), out var handlerInfos))
            _handlersDict[typeof(TEvent)] = handlerInfos = new();

        handlerInfos.Handlers = Delegate.Combine(handlerInfos.Handlers, handler);
        //record only once handlers
        if (flags.HasFlag(HandlerSubscribeFlag.OnlyOnce)) handlerInfos.OneTimeHandlers.Add(handler);
        return () => Unsubscribe(handler);
    }

    public void Unsubscribe<TEvent>(Action<TEvent> handler)
        where TEvent : BaseEvent
    {
        if (!_handlersDict.TryGetValue(typeof(TEvent), out var handlerInfos))
            _handlersDict[typeof(TEvent)] = handlerInfos = new();

        handlerInfos.Handlers = Delegate.Remove(handlerInfos.Handlers, handler);
    }

    private void EmitEvent<TEvent>(TEvent @event) where TEvent : BaseEvent
    {
        var type = typeof(TEvent);
        List<Exception> exceptions = [];

        //遍历Event类型基类直至BaseEvent(其基类是object)
        while (type != typeof(object))
        {
            if (_handlersDict.TryGetValue(type!, out var handlerInfos))
            {
                //监听者产生的异常集中处理
                //todo: 异步处理器产生的异常没法处理，是否应考虑要求处理器返回一个task
                try
                {
                    var handlerDelegate = handlerInfos.Handlers as Action<TEvent>;
                    handlerDelegate?.Invoke(@event);
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }

                //移除一次性Handlers
                foreach (var oneTimeHandler in handlerInfos.OneTimeHandlers)
                {
                    handlerInfos.Handlers = Delegate.Remove(handlerInfos.Handlers, oneTimeHandler);
                }
            }

            type = type!.BaseType;
        }

        if (exceptions.Count > 0) throw new EventHandlingException(exceptions, @event);
    }

    protected abstract void EventReceived();

    protected virtual void NotifyEnqueue(Action action)
    {
        NotifyQueue.Enqueue(action);
    }

    public void Publish<TEvent>(TEvent @event) where TEvent : BaseEvent
    {
        NotifyEnqueue(() => EmitEvent(@event));
        EventReceived();
    }

    public EventTask<TEvent> PublishAndWaitFor<TEvent>(TEvent @event) where TEvent : BaseEvent
    {
        return new(@event, task =>
        {
            NotifyEnqueue(() =>
            {
                EmitEvent(task.Event);
                task.Complete();
            });
            EventReceived();
        });
    }


    private class HandlerInfos
    {
        public Delegate? Handlers { get; set; }
        public List<Delegate> OneTimeHandlers { get; } = [];
    }
}
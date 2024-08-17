namespace KirisameLib.Events;

public static class EventBus
{
    private static class HandlerContainer<TEvent> where TEvent : BaseEvent
    {
        private static List<Action<TEvent>> EventHandlers { get; } = [];

        public static void AddEventHandler(Action<TEvent> handler)
        {
            if (EventHandlers.Contains(handler)) return;
            EventHandlers.Add(handler);
        }

        public static void RemoveEventHandler(Action<TEvent> handler) => EventHandlers.Remove(handler);

        public static void InvokeHandler(TEvent @event)
        {
            foreach (var eventHandler in EventHandlers)
            {
                eventHandler.Invoke(@event);
            }
        }
    }


    /// <returns>
    ///     Callback to remove registered handler, useful when unregistering an anonymous function
    /// </returns>
    public static Action Subscribe<TEvent>(Action<TEvent> handler)
        where TEvent : BaseEvent
    {
        HandlerContainer<TEvent>.AddEventHandler(handler);
        return () => HandlerContainer<TEvent>.RemoveEventHandler(handler);
    }

    public static void Unsubscribe<TEvent>(Action<TEvent> handler)
        where TEvent : BaseEvent
    {
        HandlerContainer<TEvent>.RemoveEventHandler(handler);
    }

    public static void Publish<TEvent>(TEvent @event)
        where TEvent : BaseEvent
    {
        var type = typeof(TEvent);
        for (;;)
        {
            var handlerContainerType = typeof(HandlerContainer<>).MakeGenericType(type!);
            var invoke = handlerContainerType.GetMethod(nameof(HandlerContainer<BaseEvent>.InvokeHandler));
            invoke!.Invoke(null, [@event]);
            if (type == typeof(BaseEvent)) break;
            type = type!.BaseType;
        }
    }
}
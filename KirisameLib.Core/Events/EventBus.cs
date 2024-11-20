using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

using KirisameLib.Core.Extensions;

namespace KirisameLib.Core.Events;

//todo: 生成器就生成一个接受总线实例的注册方法
//      然后静态的部分也这样，之后生成一个总和的类来注册所有同一标记的静态事件处理器的方法（事件处理器标签上加个字符串参数应该就可以）
public class EventBus
{
    private readonly Dictionary<Type, Delegate?> _handlerDict = new();

    /// <returns>
    ///     Callback to remove registered handler, useful when unregistering an anonymous function
    /// </returns>
    public Action Subscribe<TEvent>(Action<TEvent> handler)
        where TEvent : BaseEvent
    {
        var handlerDelegate = _handlerDict.GetValueOrDefault(typeof(TEvent)) as Action<TEvent>;
        _handlerDict[typeof(TEvent)] = Delegate.Combine(handlerDelegate, handler);
        return () => Unsubscribe(handler);
    }

    public void Unsubscribe<TEvent>(Action<TEvent> handler)
        where TEvent : BaseEvent
    {
        var handlerDelegate = _handlerDict.GetValueOrDefault(typeof(TEvent)) as Action<TEvent>;
        _handlerDict[typeof(TEvent)] = Delegate.Remove(handlerDelegate, handler);
    }


    private bool _handlingEvent = false;
    private Queue<Action> _eventQueue = [];

    private void EmitEvent<TEvent>(TEvent @event) where TEvent : BaseEvent
    {
        var type = typeof(TEvent);
        List<Exception> exceptions = [];
        
        //遍历Event类型基类直至BaseEvent(其基类是object)
        while (type != typeof(object))
        {
            //监听者产生的异常集中处理
            try
            {
                var handlerDelegate = _handlerDict.GetValueOrDefault(type!) as Action<TEvent>;
                handlerDelegate?.Invoke(@event);
            }
            catch (Exception e)
            {
                exceptions.Add(e);
            }

            type = type!.BaseType;
        }
        
        if (exceptions.Count > 0) throw new EventHandlingException(exceptions, @event);
    }

    private void HandleEventQueue<TEvent>() where TEvent : BaseEvent
    {
        _handlingEvent = true;
        List<EventHandlingException> exceptions = [];
        
        while (_eventQueue.TryDequeue(out var eventAction))
        {
            try
            {
                eventAction.Invoke();
            }
            catch (EventHandlingException e)
            {
                exceptions.Add(e);
            }
        }
        _handlingEvent = false;

        if (exceptions.Count > 0) throw new QueueEventHandlingException(exceptions);
    }
    
    public void Publish<TEvent>(TEvent @event) where TEvent : BaseEvent
    {
        _eventQueue.Enqueue(() => EmitEvent(@event));
        if (_handlingEvent) return;

        HandleEventQueue<TEvent>();
    }


    public class EventHandlingException(IEnumerable<Exception> innerExceptions, BaseEvent @event) : AggregateException(innerExceptions)
    {
        public BaseEvent FromEvent => @event;
    }

    public class QueueEventHandlingException(IEnumerable<EventHandlingException> innerExceptions) : AggregateException(innerExceptions)
    {
        [field: AllowNull, MaybeNull]
        public ReadOnlyCollection<EventHandlingException> EventHandlingExceptions => field ??= new(innerExceptions.ToList());
    }
}
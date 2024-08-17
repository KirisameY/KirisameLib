using System.Reflection;

using KirisameLib.Logging;

namespace KirisameLib.Events;

public static class EventHandlerSubscriber
{
    static EventHandlerSubscriber()
    {
        var assembly = Assembly.GetAssembly(typeof(EventHandlerSubscriber));
        if (assembly is not null) SubscribeStaticIn(assembly);
    }


    public static void InstanceSubscribe(object container) => InstanceSubscribe(container,   true);
    public static void InstanceUnsubscribe(object container) => InstanceSubscribe(container, false);

    private static void InstanceSubscribe(object container, bool register)
    {
        var methodList =
            from method in container.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            where method.CustomAttributes.Any(data => data.AttributeType == typeof(EventHandlerAttribute))
            select method;

        foreach (var method in methodList)
        {
            if (method.ReturnType != typeof(void))
            {
                Logger.Log(LogLevel.Error, nameof(InstanceSubscribe),
                           $"Method {container.GetType().Name}.{method} has EventHandlerAttribute, but return type is not void");
                continue;
            }

            if (method.GetParameters().Length != 1)
            {
                Logger.Log(LogLevel.Error, nameof(InstanceSubscribe),
                           $"Method {container.GetType().Name}.{method} has EventHandlerAttribute, but parameters count is not 1");
                continue;
            }

            var eventType = method.GetParameters()[0].ParameterType;
            if (eventType.IsGenericType)
            {
                Logger.Log(LogLevel.Error, nameof(InstanceSubscribe),
                           $"Method {container.GetType().Name}.{method} has EventHandlerAttribute, but parameters type is in generic");
                continue;
            }

            if (!typeof(BaseEvent).IsAssignableFrom(eventType))
            {
                Logger.Log(LogLevel.Error, nameof(InstanceSubscribe),
                           $"Method {container.GetType().Name}.{method} has EventHandlerAttribute, but parameters type is not event");
                continue;
            }

            var delegateType = typeof(Action<>).MakeGenericType(eventType);
            var delegateInstance = method.CreateDelegate(delegateType, container);
            if (register)
                typeof(EventBus).GetMethod(nameof(EventBus.Subscribe))!.MakeGenericMethod(eventType).Invoke(null, [delegateInstance]);
            else
                typeof(EventBus).GetMethod(nameof(EventBus.Unsubscribe))!.MakeGenericMethod(eventType).Invoke(null, [delegateInstance]);
        }
    }


    public static void SubscribeStaticIn(Assembly assembly)
    {
        var types =
            from type in assembly.GetTypes()
            where type.CustomAttributes.Any(data => data.AttributeType == typeof(EventHandlerAttribute))
            select type;

        foreach (Type type in types)
            StaticSubscribe(type);
    }

    public static void StaticSubscribe(Type type)
    {
        var methodEventList =
            from method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
            where method.CustomAttributes.Any(data => data.AttributeType == typeof(EventHandlerAttribute))
            select method;

        foreach (MethodInfo method in methodEventList)
        {
            if (method.ReturnType != typeof(void))
            {
                Logger.Log(LogLevel.Error, nameof(StaticSubscribe),
                           $"Method {type.Name}.{method} has EventHandlerAttribute, but return type is not void");
                continue;
            }

            if (method.GetParameters().Length != 1)
            {
                Logger.Log(LogLevel.Error, nameof(StaticSubscribe),
                           $"Method {type.Name}.{method} has EventHandlerAttribute, but parameters count is not 1");
                continue;
            }

            var eventType = method.GetParameters()[0].ParameterType;
            if (eventType.IsGenericType)
            {
                Logger.Log(LogLevel.Error, nameof(StaticSubscribe),
                           $"Method {type.Name}.{method} has EventHandlerAttribute, but parameters type is in generic");
                continue;
            }

            if (!typeof(BaseEvent).IsAssignableFrom(eventType))
            {
                Logger.Log(LogLevel.Error, nameof(StaticSubscribe),
                           $"Method {type.Name}.{method} has EventHandlerAttribute, but parameters type is not event");
                continue;
            }

            var delegateType = typeof(Action<>).MakeGenericType(eventType);
            var delegateInstance = method.CreateDelegate(delegateType);
            typeof(EventBus).GetMethod(nameof(EventBus.Subscribe))!.MakeGenericMethod(eventType).Invoke(null, [delegateInstance]);
        }
    }

    //Logging
    private static Logger Logger { get; } = LogManager.GetLogger(nameof(EventHandlerSubscriber));
}
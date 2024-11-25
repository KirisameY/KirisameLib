using System.Collections.Frozen;
using System.Collections.Immutable;

using KirisameLib.GeneratorTools;
using KirisameLib.GeneratorTools.Extensions;
using KirisameLib.GeneratorTools.Utils;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace KirisameLib.Core.SourceGenerator.Generators;

[Generator]
public class EventHandlerGenerator : IIncrementalGenerator
{
    private static class Names
    {
        public const string EventHandlerContainerAttribute = "KirisameLib.Core.Events.EventHandlerContainerAttribute";
        public const string EventHandlerAttribute = "KirisameLib.Core.Events.EventHandlerAttribute";

        public const string EventBus = "KirisameLib.Core.Events.EventBus";
        public const string BaseEvent = "KirisameLib.Core.Events.BaseEvent";
        public const string HandlerSubscribeFlag = "KirisameLib.Core.Events.HandlerSubscribeFlag";

        public const string SourceFileNameSuffix = "_EventHandlerSubscriber.generated.cs";
        public const string GlobalClassFileName = "GlobalEventHandlersSubscriber.generated.cs";
        public const string GlobalClassNamespace = "KirisameLib.Core.Events.Generated";
        public const string GlobalClassName = "GlobalEventHandlersSubscriber";
    }


    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classInfoProvider = context.SyntaxProvider
                                       .CreateSyntaxProvider(EventHandlerClassPredicate, EventHandlerClassSymbolTransform)
                                       .WhereNotNull()
                                       .Select(EventHandlerClassInfoTransform);

        var globalInfoProvider = classInfoProvider.Collect()
                                                  .Select(SelectGlobalHandlersInfo);

        context.RegisterSourceOutput(classInfoProvider,  GenerateForClass);
        context.RegisterSourceOutput(globalInfoProvider, GenerateForGlobal);
    }


    #region SelectClassInfo

    private record struct EventHandlerInfo(string MethodName, string EventType, int FlagsValue);

    private record struct EventContainerClassInfo(
        INamedTypeSymbol ClassSymbol,
        bool Static,
        bool Inherited,
        FrozenDictionary<string, List<EventHandlerInfo>> StaticEventHandlers,
        FrozenDictionary<string, List<EventHandlerInfo>> InstanceEventHandlers
    );

    private static bool EventHandlerClassPredicate(SyntaxNode node, CancellationToken _) => node is ClassDeclarationSyntax;

    private static INamedTypeSymbol? EventHandlerClassSymbolTransform(GeneratorSyntaxContext context, CancellationToken _)
    {
        var model = context.SemanticModel;
        var classDeclaration = (ClassDeclarationSyntax)context.Node;
        var classSymbol = model.GetDeclaredSymbol(classDeclaration) as INamedTypeSymbol;

        return classSymbol!.GetAttributes().Any(a => a.AttributeClass!.IsDerivedFrom(Names.EventHandlerContainerAttribute))
            ? classSymbol : null;
    }

    private static EventContainerClassInfo EventHandlerClassInfoTransform(INamedTypeSymbol classSymbol, CancellationToken _)
    {
        bool isStatic = classSymbol.IsStatic;

        bool inherited = classSymbol is
        {
            IsStatic: false,
            BaseType: { } baseType
        } && baseType.GetAttributes().Any(a => a.AttributeClass!.IsDerivedFrom(Names.EventHandlerContainerAttribute));

        //methods
        Dictionary<string, List<EventHandlerInfo>> staticHandlers = [],
                                                   instanceHandlers = [];

        foreach (var method in classSymbol.GetMembers().OfType<IMethodSymbol>())
        {
            if (method.IsOverride) continue;   //do not consider inherited method
            if (!method.ReturnsVoid) continue; //should be void return type
            //should have single event parameter
            if (method.Parameters is not [{ Type: INamedTypeSymbol paramType }] || !paramType.IsDerivedFrom(Names.BaseEvent)) continue;

            foreach (var attribute in method.GetAttributes())
            {
                if (!attribute.AttributeClass!.IsDerivedFrom(Names.EventHandlerAttribute)) continue;
                var targetDict = method.IsStatic ? staticHandlers : instanceHandlers;
                var handlerName = method.Name;
                var eventType = paramType.ToDisplayString();

                if (attribute.ConstructorArguments is not [{ Values: var groupArg }, { Value: int flagsArg }])
                    throw new InvalidOperationException();

                string[] groups = groupArg.IsDefaultOrEmpty ? [""] : groupArg.SelectNotNull(c => c.Value as string).ToArray();
                int flagsValue = Convert.ToInt32(flagsArg);

                //add info
                foreach (var group in groups)
                {
                    if (!targetDict.TryGetValue(group, out var list)) targetDict[group] = list = [];
                    list.Add(new(handlerName, eventType, flagsValue));
                }
                break;
            }
        }

        return new(classSymbol, isStatic, inherited, staticHandlers.ToFrozenDictionary(), instanceHandlers.ToFrozenDictionary());
    }

    #endregion


    #region SelectGlobalInfo

    private record struct GlobalStaticHandlersInfo(string ClassFullName);

    private static GlobalStaticHandlersInfo[] SelectGlobalHandlersInfo(ImmutableArray<EventContainerClassInfo> eventContainerClassInfos,
                                                                       CancellationToken cancellationToken)
    {
        var result = eventContainerClassInfos.Where(info => info.StaticEventHandlers.Count > 0)
                                             .Select(info => info.ClassSymbol.ToDisplayString())
                                             .Select(name => new GlobalStaticHandlersInfo(name));
        return result.ToArray();
    }

    #endregion


    #region Excution

    private static void GenerateForClass(SourceProductionContext context, EventContainerClassInfo classInfo)
    {
        var classFullName = classInfo.ClassSymbol.ToDisplayString();
        var className = classInfo.ClassSymbol.Name;
        var namespaceName = classInfo.ClassSymbol.ContainingNamespace.ToDisplayString();


        var sourceBuilder = new IndentStringBuilder();

        sourceBuilder.AppendLine($"namespace {namespaceName};")
                     .AppendLine();
        sourceBuilder.AppendLine("#pragma warning disable CS1522 // Empty switch block")
                     .AppendLine("#pragma warning disable CS0162 // Unreachable code detected")
                     .Append("partial ");
        if (classInfo.Static) sourceBuilder.Append("static ");
        sourceBuilder.AppendLine($"class {className}")
                     .AppendLine("{");
        using (sourceBuilder.Indent())
        {
            #region Static Subscribe

            sourceBuilder.AppendLine(Global.GeneratedCodeAttribute)
                         .Append(classInfo.Inherited ? "internal new " : "internal ")
                         .AppendLine($"static void SubscribeStaticHandler(global::{Names.EventBus} bus, string group = \"\")")
                         .AppendLine("{");
            using (sourceBuilder.Indent())
            {
                sourceBuilder.AppendLine("switch (group)")
                             .AppendLine("{")
                             .IncreaseIndent();
                foreach (var (group, handlers) in classInfo.StaticEventHandlers)
                {
                    sourceBuilder.AppendLine($"case \"{group}\":");
                    using (sourceBuilder.Indent())
                    {
                        foreach (var (methodName, eventType, flagsValue) in handlers)
                        {
                            sourceBuilder.AppendLine
                                ($"bus.Subscribe<global::{eventType}>({methodName}, (global::{Names.HandlerSubscribeFlag}){flagsValue});");
                        }
                        sourceBuilder.AppendLine("break;");
                    }
                }
                sourceBuilder.DecreaseIndent()
                             .AppendLine("}");
            }
            sourceBuilder.AppendLine("}");

            #endregion

            #region Static Unubscribe

            sourceBuilder.AppendLine()
                         .AppendLine(Global.GeneratedCodeAttribute)
                         .Append(classInfo.Inherited ? "internal new " : "internal ")
                         .AppendLine($"static void UnsubscribeStaticHandler(global::{Names.EventBus} bus, string group = \"\")")
                         .AppendLine("{");
            using (sourceBuilder.Indent())
            {
                sourceBuilder.AppendLine("switch (group)")
                             .AppendLine("{")
                             .IncreaseIndent();
                foreach (var (group, handlers) in classInfo.StaticEventHandlers)
                {
                    sourceBuilder.AppendLine($"case \"{group}\":");
                    using (sourceBuilder.Indent())
                    {
                        foreach (var (methodName, eventType, _) in handlers)
                        {
                            sourceBuilder.AppendLine
                                ($"bus.Unsubscribe<global::{eventType}>({methodName});");
                        }
                        sourceBuilder.AppendLine("break;");
                    }
                }
                sourceBuilder.DecreaseIndent()
                             .AppendLine("}");
            }
            sourceBuilder.AppendLine("}");

            #endregion

            #region Instance Subscribe

            sourceBuilder.AppendLine()
                         .AppendLine(Global.GeneratedCodeAttribute)
                         .Append(classInfo.Inherited ? "protected override " : "protected virtual ")
                         .AppendLine($"void SubscribeInstanceHandler(global::{Names.EventBus} bus, string group = \"\")")
                         .AppendLine("{");
            using (sourceBuilder.Indent())
            {
                sourceBuilder.AppendLine("switch (group)")
                             .AppendLine("{")
                             .IncreaseIndent();
                foreach (var (group, handlers) in classInfo.InstanceEventHandlers)
                {
                    sourceBuilder.AppendLine($"case \"{group}\":");
                    using (sourceBuilder.Indent())
                    {
                        foreach (var (methodName, eventType, flagsValue) in handlers)
                        {
                            sourceBuilder.AppendLine
                                ($"bus.Subscribe<global::{eventType}>({methodName}, (global::{Names.HandlerSubscribeFlag}){flagsValue});");
                        }
                        sourceBuilder.AppendLine("break;");
                    }
                }
                sourceBuilder.DecreaseIndent()
                             .AppendLine("}");
                if (classInfo.Inherited) sourceBuilder.AppendLine("base.SubscribeInstanceHandler(bus, group);");
            }
            sourceBuilder.AppendLine("}");

            #endregion

            #region Instance Unsubscribe

            sourceBuilder.AppendLine()
                         .AppendLine(Global.GeneratedCodeAttribute)
                         .Append(classInfo.Inherited ? "protected override " : "protected virtual ")
                         .AppendLine($"void UnsubscribeInstanceHandler(global::{Names.EventBus} bus, string group = \"\")")
                         .AppendLine("{");
            using (sourceBuilder.Indent())
            {
                sourceBuilder.AppendLine("switch (group)")
                             .AppendLine("{")
                             .IncreaseIndent();
                foreach (var (group, handlers) in classInfo.InstanceEventHandlers)
                {
                    sourceBuilder.AppendLine($"case \"{group}\":");
                    using (sourceBuilder.Indent())
                    {
                        foreach (var (methodName, eventType, _) in handlers)
                        {
                            sourceBuilder.AppendLine
                                ($"bus.Unsubscribe<global::{eventType}>({methodName});");
                        }
                        sourceBuilder.AppendLine("break;");
                    }
                }
                sourceBuilder.DecreaseIndent()
                             .AppendLine("}");
                if (classInfo.Inherited) sourceBuilder.AppendLine("base.UnsubscribeInstanceHandler(bus, group);");
            }
            sourceBuilder.AppendLine("}");

            #endregion
        }
        sourceBuilder.AppendLine("}");


        context.AddSource($"{classFullName}{Names.SourceFileNameSuffix}", sourceBuilder.ToString());
    }

    private static void GenerateForGlobal(SourceProductionContext context, GlobalStaticHandlersInfo[] globalStaticHandlersInfos)
    {
        IndentStringBuilder sourceBuilder = new();

        sourceBuilder.AppendLine($"namespace {Names.GlobalClassNamespace};")
                     .AppendLine();
        sourceBuilder.AppendLine(Global.GeneratedCodeAttribute)
                     .AppendLine($"internal static class {Names.GlobalClassName}")
                     .AppendLine("{");
        using (sourceBuilder.Indent())
        {
            #region Subscribe

            sourceBuilder.AppendLine($"public static void Subscribe(global::{Names.EventBus} bus, string group = \"\")")
                         .AppendLine("{");
            using (sourceBuilder.Indent())
            {
                foreach (var info in globalStaticHandlersInfos)
                {
                    sourceBuilder.AppendLine($"global::{info.ClassFullName}.SubscribeStaticHandler(bus, group);");
                }
            }
            sourceBuilder.AppendLine("}");

            #endregion

            #region Unsubscribe

            sourceBuilder.AppendLine()
                         .AppendLine($"public static void Unsubscribe(global::{Names.EventBus} bus, string group = \"\")")
                         .AppendLine("{");
            using (sourceBuilder.Indent())
            {
                foreach (var info in globalStaticHandlersInfos)
                {
                    sourceBuilder.AppendLine($"global::{info.ClassFullName}.UnsubscribeStaticHandler(bus, group);");
                }
            }
            sourceBuilder.AppendLine("}");

            #endregion
            
        }
        sourceBuilder.AppendLine("}");

        context.AddSource(Names.GlobalClassFileName, sourceBuilder.ToString());
    }

    #endregion
}
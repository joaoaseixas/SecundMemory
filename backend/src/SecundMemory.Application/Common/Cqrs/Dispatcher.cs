using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace SecundMemory.Application.Common.Cqrs;

public sealed class Dispatcher(IServiceProvider serviceProvider) : IDispatcher
{
    private static readonly ConcurrentDictionary<Type, MethodInfo> HandleMethodCache = new();

    public Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default) =>
        SendInternal<TResponse>(command, typeof(ICommandHandler<,>), cancellationToken);

    public Task<TResponse> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default) =>
        SendInternal<TResponse>(query, typeof(IQueryHandler<,>), cancellationToken);

    private Task<TResponse> SendInternal<TResponse>(object request, Type openHandlerType, CancellationToken cancellationToken)
    {
        var requestType = request.GetType();
        var handlerType = openHandlerType.MakeGenericType(requestType, typeof(TResponse));
        var handler = serviceProvider.GetRequiredService(handlerType);
        var handleMethod = GetHandleMethod(handlerType);

        RequestHandlerDelegate<TResponse> callHandler = () =>
            (Task<TResponse>)handleMethod.Invoke(handler, [request, cancellationToken])!;

        var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, typeof(TResponse));
        var behaviorHandleMethod = GetHandleMethod(behaviorType);
        var behaviors = serviceProvider.GetServices(behaviorType).Reverse();

        var pipeline = behaviors.Aggregate(callHandler, (next, behavior) =>
            () => (Task<TResponse>)behaviorHandleMethod.Invoke(behavior, [request, next, cancellationToken])!);

        return pipeline();
    }

    private static MethodInfo GetHandleMethod(Type type) =>
        HandleMethodCache.GetOrAdd(type, t => t.GetMethod("Handle")
            ?? throw new InvalidOperationException($"Type '{t}' has no Handle method."));
}

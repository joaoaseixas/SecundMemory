namespace SecundMemory.Application.Common.Cqrs;

public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();

/// <summary>
/// Wraps a command/query handler — Validation, Logging, Transaction and
/// Performance behaviors are all implementations of this, composed in
/// registration order around the real handler by Dispatcher.
/// </summary>
public interface IPipelineBehavior<TRequest, TResponse>
{
    Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken);
}

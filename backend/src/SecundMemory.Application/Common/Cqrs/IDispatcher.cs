namespace SecundMemory.Application.Common.Cqrs;

/// <summary>The only thing Api controllers depend on from Application — never a concrete handler.</summary>
public interface IDispatcher
{
    Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);
    Task<TResponse> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);
}

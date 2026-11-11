using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Application.Common.Cqrs;

namespace SecundMemory.Application.Common.Behaviors;

/// <summary>
/// Centralizes "save after a successful command" so handlers don't each have
/// to remember to call SaveChangesAsync. Not a multi-request ACID wrapper —
/// IUnitOfWork exposes a single SaveChangesAsync, which EF Core already makes
/// atomic for everything changed during one command's handling. Queries pass
/// straight through: nothing to persist, nothing to wrap.
/// </summary>
public class TransactionBehavior<TRequest, TResponse>(IUnitOfWork unitOfWork) : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not ICommand<TResponse>) return await next();

        var response = await next();
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return response;
    }
}

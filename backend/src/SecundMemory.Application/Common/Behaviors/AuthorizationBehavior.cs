using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Common.Exceptions;
using SecundMemory.Application.Common.Security;

namespace SecundMemory.Application.Common.Behaviors;

/// <summary>Enforces [Authorize] on a Command/Query — see Common.Security.AuthorizeAttribute.</summary>
public class AuthorizationBehavior<TRequest, TResponse>(ICurrentUser currentUser) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var attribute = (AuthorizeAttribute?)Attribute.GetCustomAttribute(typeof(TRequest), typeof(AuthorizeAttribute));
        if (attribute is null) return await next();

        if (!currentUser.IsAuthenticated)
            throw new UnauthorizedAccessException($"{typeof(TRequest).Name} requires an authenticated user.");

        if (attribute.Roles.Length > 0 && !attribute.Roles.Any(role => currentUser.Roles.Contains(role)))
            throw new ForbiddenException($"{typeof(TRequest).Name} requires one of the roles: {string.Join(", ", attribute.Roles)}.");

        return await next();
    }
}

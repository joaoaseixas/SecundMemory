namespace SecundMemory.Application.Common.Security;

/// <summary>
/// Marks a Command/Query as requiring an authenticated user, optionally with
/// specific roles. Enforced by AuthorizationBehavior in the dispatch
/// pipeline — framework-agnostic, unlike ASP.NET Core's own [Authorize].
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class AuthorizeAttribute : Attribute
{
    public string[] Roles { get; init; } = [];
}

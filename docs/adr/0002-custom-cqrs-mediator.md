# ADR 0002: Hand-rolled CQRS mediator instead of MediatR

## Status
Accepted

## Context
The requested Application layer structure uses a CQRS shape — Commands, Queries,
Handlers, and pipeline Behaviors (Validation/Logging/Transaction/Performance) —
which is the shape MediatR provides out of the box in most .NET Clean
Architecture templates.

MediatR (and AutoMapper, same author) moved to a commercial licensing model for
new major versions in 2024, with revenue-based terms for companies above a
threshold. This project is a public, open-source portfolio repository; pulling
in a dependency with an evolving commercial license adds a risk that isn't
worth it just to get the dispatch pattern, which is small enough to own
directly.

## Decision
Implement the same shape ourselves in `SecundMemory.Application.Common`:

- `ICommand<TResponse>` / `IQuery<TResponse>` — marker request interfaces.
- `ICommandHandler<TCommand, TResponse>` / `IQueryHandler<TQuery, TResponse>`.
- `IDispatcher` — resolves the right handler from DI and invokes it, wrapped by
  a pipeline of `IPipelineBehavior<TRequest, TResponse>` (same concept as
  MediatR's, just owned code).
- `Behaviors/ValidationBehavior`, `LoggingBehavior`, `TransactionBehavior`,
  `PerformanceBehavior` — implemented against our own `IPipelineBehavior`.

Registration is reflection-based over `SecundMemory.Application`'s assembly,
same as `AddMediatR(...)` would do, via `DependencyInjection.AddApplication()`.

## Consequences
- ~150 lines of infrastructure code we own and can read end to end — no black
  box, no external license to track.
- API surface intentionally mirrors MediatR's naming so the mental model (and
  any future migration to MediatR, if the license risk becomes moot) stays
  cheap.
- FluentValidation is kept as a real dependency — it has no equivalent
  licensing concern.

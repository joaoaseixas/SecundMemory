# Architecture

The backend follows Robert C. Martin's **Clean Architecture**: four concentric
layers, one project each, with a single rule governing every dependency between
them.

## The Dependency Rule

> Source code dependencies can only point **inwards**. Nothing in an inner
> circle can know anything about an outer circle.

```
                    ┌─────────────────────────────────────────────┐
                    │  Frameworks & Drivers                        │
                    │  SecundMemory.Api                            │
                    │  ASP.NET Core · controllers · MCP endpoint   │
                    │  ┌───────────────────────────────────────┐  │
                    │  │  Interface Adapters                   │  │
                    │  │  SecundMemory.Infrastructure           │  │
                    │  │  EF Core · Npgsql · ApplicationDbContext│  │
                    │  │  ┌─────────────────────────────────┐  │  │
                    │  │  │  Use Cases                        │  │  │
                    │  │  │  SecundMemory.Application          │  │  │
                    │  │  │  NoteService · ProfileFactService  │  │  │
                    │  │  │  CapacityService                   │  │  │
                    │  │  │  ┌───────────────────────────┐    │  │  │
                    │  │  │  │  Entities                  │    │  │  │
                    │  │  │  │  SecundMemory.Domain         │    │  │  │
                    │  │  │  │  Note · Tag · ProfileFact   │    │  │  │
                    │  │  │  └───────────────────────────┘    │  │  │
                    │  │  └─────────────────────────────────┘  │  │
                    │  └───────────────────────────────────────┘  │
                    └─────────────────────────────────────────────┘
              dependencies point inward  →  Api → Infrastructure → Application → Domain
```

| Book's term            | Project                       | Contains                                                              | Depends on |
|-------------------------|--------------------------------|-------------------------------------------------------------------------|------------|
| Entities                | `SecundMemory.Domain`          | `Note`, `Tag`, `ProfileFact` — plain C# classes, no package references   | nothing |
| Use Cases               | `SecundMemory.Application`     | `NoteService`, `ProfileFactService`, `CapacityService`, and the `IApplicationDbContext` port they're coded against | Domain only |
| Interface Adapters      | `SecundMemory.Infrastructure`  | `ApplicationDbContext` (EF Core), Npgsql wiring, migrations              | Application, Domain |
| Frameworks & Drivers    | `SecundMemory.Api`             | ASP.NET Core composition root, controllers, the MCP server endpoint     | Application, Infrastructure |

The critical consequence: **`SecundMemory.Application` has no reference to EF
Core's Npgsql provider, ASP.NET Core, or anything else framework-specific.**
It depends only on the `IApplicationDbContext` interface it declares itself
(`Application/Common/Interfaces/IApplicationDbContext.cs`). `Infrastructure`
is the only project that knows Postgres exists.

## Why this matters here specifically

Because `Application` doesn't depend on `Infrastructure`, the use cases can be
tested with a fake in-memory `IApplicationDbContext` and never need your real
cloud Postgres instance to be reachable — see `tests/SecundMemory.Application.UnitTests`.
If you ever swap Postgres for something else, only `Infrastructure` changes;
`Domain`, `Application`, and the controllers in `Api` do not.

## Tests mirror the layers

```
tests/
├── SecundMemory.Domain.UnitTests/           # Entities in isolation, zero dependencies
├── SecundMemory.Application.UnitTests/      # Use cases against a fake IApplicationDbContext (EF Core InMemory)
└── SecundMemory.Infrastructure.IntegrationTests/  # Real ApplicationDbContext mapping against a relational engine (SQLite), no live Postgres required
```

Run them all from `backend/`:

```
dotnet test
```

## Solution layout

`backend/SecundMemory.slnx` groups the seven projects by layer so this is
visible in any IDE, not just on disk:

```
/src/Core/            Domain, Application     (the two layers with zero outward dependencies)
/src/Infrastructure/  Infrastructure
/src/Presentation/    Api
/tests/               the three test projects above
```

See the root `README.md` for how the frontend (`frontend/secund-memory-web`)
is organized and how the two sides talk to each other.

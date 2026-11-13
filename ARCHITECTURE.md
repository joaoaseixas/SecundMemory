# Architecture

The backend follows Robert C. Martin's **Clean Architecture**: four concentric
layers, one project each, with a single rule governing every dependency between
them.

## The Dependency Rule

> Source code dependencies can only point **inwards**. Nothing in an inner
> circle can know anything about an outer circle.

```
              Api  →  Infrastructure  →  Application  →  Domain
```

| Layer                | Project                       | Contains | Depends on |
|----------------------|-------------------------------|----------|------------|
| Entities             | `SecundMemory.Domain`         | Aggregates and value objects: `Page`, `Block`, `Tag`, `Journal`, `Attachment`, `User`, `Workspace`; link parsing (`LinkParser`, `BlockReference`); domain events; `Result`, `ValueObject`, soft-delete/audit base types. No package references. | nothing |
| Use Cases            | `SecundMemory.Application`    | Commands/queries and their handlers (feature folders), validators, DTOs, pipeline behaviors, and the ports (`IApplicationDbContext`, `ISearchService`, `IFileStorage`, `IJwtTokenService`, `ICacheService`, ...). | Domain |
| Interface Adapters   | `SecundMemory.Infrastructure` | EF Core + Npgsql `ApplicationDbContext` and migrations, Postgres full-text search, JWT/password hashing, file storage (local and S3), memory cache, background jobs. | Application, Domain |
| Frameworks & Drivers | `SecundMemory.Api`            | ASP.NET Core composition root: controllers, exception/logging middleware, SignalR hub, MCP server, Swagger, optional SPA hosting. | Application, Infrastructure |

`Application` never references EF Core's Npgsql provider, ASP.NET Core or any other
framework: it only knows the ports it declares itself. `Infrastructure` is the only
project that knows Postgres exists. These rules are enforced by
`SecundMemory.ArchitectureTests`.

## Application layer: CQRS without a library

Use cases are organised by feature (`Features/Pages`, `Blocks`, `Journals`, `Tags`,
`Graph`, `Search`, `Attachments`, `Auth`, `ImportExport`), each split into
`Commands/` and `Queries/`. A small in-house `IDispatcher` (see
[ADR 0002](docs/adr/0002-custom-cqrs-mediator.md)) resolves the handler and wraps it
in a pipeline of behaviors, outermost first:

```
Logging → Authorization → Validation → Performance → Transaction → Handler
```

Domain events raised by aggregates are dispatched after the transaction's changes
are saved (`IDomainEventDispatcher`). Examples: indexing `[[links]]` and `#tags`
when block content changes, and rewriting references when a page is renamed.

Objects are mapped by hand rather than with AutoMapper
([ADR 0003](docs/adr/0003-custom-mapping-over-automapper.md)).

## Data and multi-tenancy

Everything belongs to a `Workspace`; the workspace id is carried in the JWT and read
through `ICurrentUser`. Entities are auditable and soft-deleted; a background job
(`CleanupSoftDeletedJob`) purges old deleted rows, and `ReindexSearchJob` rebuilds
the search index. Tags are their own entity, not unified with pages
([ADR 0001](docs/adr/0001-tag-as-separate-entity.md)).

Your data lives in a Postgres database you own. The connection string is supplied via
user-secrets or the `ConnectionStrings__SecondBrain` environment variable, never a
file in this repo.

## Api layer

- Controllers are thin: they send a command/query through the dispatcher.
- `ExceptionHandlingMiddleware` maps `ValidationException`, `NotFoundException`,
  `ForbiddenException` and `UnauthorizedAccessException` to HTTP responses.
- `SyncHub` (`/hubs/sync`) pushes realtime updates between clients.
- `/mcp` exposes MCP tools (`SecondBrainTools`) so AI assistants can search the vault,
  fetch backlinks and open today's journal.
- When `Spa:Root` (or `./wwwroot`) contains a built `index.html`, the API serves the
  Angular app too, so the whole product runs as one process (`run.ps1`).

## Frontend

`frontend/secund-memory-web` is a standalone-component Angular app: `core/`
(API services, auth, config), `shared/` (reusable UI), `layout/` (main and auth
layouts) and lazy-loaded `features/` — `editor`, `pages`, `journals`, `tags`,
`graph` (force-directed, on canvas), `search`, `attachments`, `import-export`,
`settings`, `auth`. The block editor is a custom textarea-based editor
([ADR 0005](docs/adr/0005-custom-textarea-editor-over-tiptap.md)).

## Tests mirror the layers

```
backend/tests/
├── SecundMemory.Domain.UnitTests/                 # Entities in isolation
├── SecundMemory.Application.UnitTests/            # Handlers against a fake IApplicationDbContext
├── SecundMemory.Infrastructure.IntegrationTests/  # Real DbContext mapping on SQLite, no live Postgres
├── SecundMemory.Api.IntegrationTests/             # HTTP pipeline end to end
└── SecundMemory.ArchitectureTests/                # Dependency rule and handler naming conventions
```

Run them all from `backend/` with `dotnet test`.

## Solution layout

`backend/SecundMemory.slnx` groups the nine projects by layer:

```
/src/Core/            Domain, Application
/src/Infrastructure/  Infrastructure
/src/Presentation/    Api
/tests/               the five test projects above
```

See the root [`README.md`](./README.md) for setup and running instructions.

# Secund Memory

A personal "second brain" — part Obsidian-style knowledge vault, part durable memory
layer that any AI assistant can plug into instead of making you re-explain yourself
every time you start a new chat or subscribe to a new AI service.

## How it's split

This repo contains **only code** — a C#/.NET backend and an Angular frontend. It's
public on purpose: it's a portfolio piece, and there is nothing personal in it.

Your actual data (notes, profile facts, preferences, anything personal) lives in a
**Postgres database that you own and control in the cloud**. The backend connects to
it at runtime via a connection string that is never stored in this repo:

- **Locally**: `dotnet user-secrets`, stored under your OS profile — completely
  outside the repo folder. Already initialized for `SecundMemory.Api`; set yours with:
  ```
  cd backend/src/SecundMemory.Api
  dotnet user-secrets set "ConnectionStrings:SecondBrain" "Host=...;Database=...;Username=...;Password=...;SSL Mode=Require"
  ```
- **Any other environment**: the `ConnectionStrings__SecondBrain` environment variable.

`.gitignore` (repo root) also blocks `appsettings.*.local.json`, `secrets/`, `data/`,
and `*.db`/`*.sqlite` as defense in depth, even though the connection string path
above never touches a file in this repo at all.

## Architecture

Full write-up (including the Clean Architecture layer mapping and the test
projects that mirror it): see [`ARCHITECTURE.md`](./ARCHITECTURE.md).


```
frontend/secund-memory-web          backend/src
┌─────────────────────┐    REST    ┌───────────────────────────┐    SQL    ┌─────────────────────┐
│ Angular (standalone) │◄──────────►│ SecundMemory.Api            │◄─────────►│ Your cloud Postgres  │
│ core / shared /      │  :5005/api │  (ASP.NET Core, net10.0)    │           │ (data lives here,    │
│ features/dashboard   │            │                              │           │  never in this repo) │
└─────────────────────┘            │  ┌─ SecundMemory.Application │           └─────────────────────┘
                                     │  │   (use cases, DTOs)       │
                                     │  ├─ SecundMemory.Infrastructure
                                     │  │   (EF Core + Npgsql)      │
                                     │  └─ SecundMemory.Domain      │
                                     │      (entities, no deps)     │
                                     │                              │
                                     │  + MCP endpoint at /mcp ─────┼──► Any MCP-compatible AI
                                     └───────────────────────────┘      (Claude, etc.) reads your
                                                                         profile/notes as context
                                                                         instead of you retyping it
```

**Backend — Clean Architecture, 4 layers, each its own project:**

- `SecundMemory.Domain` — entities (`Note`, `Tag`, `ProfileFact`). No dependencies.
- `SecundMemory.Application` — use-case services (`NoteService`, `ProfileFactService`,
  `CapacityService`) behind interfaces, plus `IApplicationDbContext` so this layer
  never depends on EF Core/Infrastructure directly.
- `SecundMemory.Infrastructure` — `ApplicationDbContext` (EF Core + Npgsql),
  migrations, DI wiring that reads the connection string from configuration.
- `SecundMemory.Api` — composition root: controllers (`/api/notes`,
  `/api/profile-facts`, `/api/capacity`) for the Angular app, and an MCP server
  (`/mcp`, via `ModelContextProtocol.AspNetCore`) exposing `get_profile`,
  `search_notes`, `get_capacity_summary` tools for AI assistants.

**Frontend — Angular (standalone components), layered by convention:**

- `core/` — singleton services (`SecondBrainApiService`), config, models.
- `shared/` — reusable UI (`StatTileComponent`).
- `features/dashboard/` — the dynamic capacity dashboard (notes/facts/tags counts).

## Running locally

```
# Backend (http://localhost:5005)
cd backend/src/SecundMemory.Api
dotnet user-secrets set "ConnectionStrings:SecondBrain" "<your cloud Postgres connection string>"
dotnet tool restore            # from backend/, installs dotnet-ef locally
dotnet ef database update --project ../SecundMemory.Infrastructure --startup-project .
dotnet run

# Frontend (http://localhost:4200)
cd frontend/secund-memory-web
npm install
npm start

# Backend tests (Domain/Application run with zero external dependencies;
# Infrastructure tests use an in-memory SQLite engine — no cloud DB needed)
cd backend
dotnet test
```

## Connecting an AI assistant via MCP

Once the API is running, point any MCP-compatible client (Claude Code, Claude
Desktop, etc.) at `http://localhost:5005/mcp` (streamable HTTP transport) to give it
`get_profile`, `search_notes`, and `get_capacity_summary` tools — so it can read who
you are and what's in your vault instead of you typing it out again.

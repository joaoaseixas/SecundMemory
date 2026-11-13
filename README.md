# Secund Memory

A personal "second brain" — a Logseq/Obsidian-style outliner of pages and nested
blocks, plus a durable memory layer that any AI assistant can plug into (via MCP)
instead of making you re-explain yourself every time you start a new chat.

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

## Features

- **Outliner editor** — pages made of nested, collapsible, movable blocks with typed
  content and block properties.
- **Daily journals** — one page per day, opened with a click (`/api/journals/today`).
- **`[[Links]]`, aliases and backlinks** — write `[[Page]]` in any block; links are
  indexed automatically, and renaming a page updates references.
- **`#tags`** — indexed automatically; browse every block carrying a tag.
- **Graph view** — force-directed graph of pages and links, with title filter and
  hide-orphans.
- **Full-text search** across pages and blocks.
- **Attachments**, **Markdown import/export**, and realtime sync (SignalR `/hubs/sync`).
- **Auth** — JWT register/login/refresh; data is scoped per workspace.
- **MCP server** at `/mcp` so AI assistants can read your vault.

## Architecture

Clean Architecture, four projects (`Domain` → `Application` → `Infrastructure` →
`Api`), with a small hand-rolled CQRS dispatcher and pipeline behaviors (logging,
authorization, validation, transactions). Full write-up in
[`ARCHITECTURE.md`](./ARCHITECTURE.md).

```
frontend/secund-memory-web          backend/src
┌─────────────────────┐    REST    ┌───────────────────────────┐    SQL    ┌─────────────────────┐
│ Angular (standalone) │◄──────────►│ SecundMemory.Api            │◄─────────►│ Your cloud Postgres  │
│ features/: editor,   │  :5005/api │  controllers + SignalR + MCP │           │ (data lives here,    │
│ pages, journals,     │            │  ├─ Application (CQRS)       │           │  never in this repo) │
│ graph, tags, search, │            │  ├─ Infrastructure (EF Core) │           └─────────────────────┘
│ attachments, settings│            │  └─ Domain                   │
└─────────────────────┘            └───────────────────────────┘
```

Backend tests live in `backend/tests` (domain, application, architecture rules,
infrastructure on in-memory SQLite, and API integration tests) — none need the cloud DB.

## Running locally

**Prerequisites:** .NET 10 SDK, Node.js + npm, a Postgres database.

**One-time setup** (secrets live in your OS profile, not in the repo):

```
cd backend/src/SecundMemory.Api
dotnet user-secrets set "ConnectionStrings:SecondBrain" "Host=...;Database=...;Username=...;Password=...;SSL Mode=Require"
dotnet user-secrets set "Jwt:Secret" "<random string, 32+ chars>"

cd ../..                       # backend/
dotnet tool restore
dotnet ef database update --project src/SecundMemory.Infrastructure --startup-project src/SecundMemory.Api
```

**Quick start (single process):** run `iniciar.bat` (or `./run.ps1`) from the repo
root. It builds the Angular app and starts the API, which also serves the UI at
http://localhost:5005 (`-SkipBuild` reuses the last build). Register an account on
the login screen and start writing.

**Dev mode (hot reload):**

```
cd backend/src/SecundMemory.Api && dotnet run        # API on :5005
cd frontend/secund-memory-web && npm install && npm start   # UI on :4200, proxies /api
```

**Tests:** `cd backend && dotnet test`

## API overview

All endpoints except `auth/*` require `Authorization: Bearer <token>`. Swagger is
available in Development.

| Area        | Endpoints |
|-------------|-----------|
| Auth        | `POST /api/auth/register`, `/login`, `/refresh` |
| Pages       | `GET/POST /api/pages`, `GET /api/pages/{id}`, `/by-title`, `/{id}/backlinks`, `PUT /{id}/title`, `DELETE /{id}` |
| Blocks      | `GET /api/pages/{id}/blocks`, `POST /api/blocks`, `PUT /api/blocks/{id}/content\|move\|collapse\|type\|properties/{key}`, `DELETE /api/blocks/{id}` |
| Journals    | `POST /api/journals/today`, `POST/GET /api/journals/{date}` |
| Tags        | `GET/POST /api/tags`, `GET /api/tags/{id}/blocks`, `POST/DELETE /api/tags/{tagId}/blocks/{blockId}` |
| Graph       | `GET /api/graph` |
| Search      | `GET /api/search` |
| Attachments | `GET/POST /api/attachments` |

## Connecting an AI assistant via MCP

With the API running, point any MCP-compatible client (Claude Code, Claude Desktop,
etc.) at `http://localhost:5005/mcp` (streamable HTTP). Tools exposed:

- `search_second_brain` — free-text search in a workspace
- `get_page_backlinks` — pages that link to a given page
- `open_today_journal` — open (or create) today's journal page

Tools take a `workspaceId` / `pageId`; you can get your workspace id from the
`register`/`login` response.

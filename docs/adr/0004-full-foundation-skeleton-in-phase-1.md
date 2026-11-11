# ADR 0004: Build the full foundation skeleton now, not per-phase

## Status
Accepted

## Context
The original phase plan (Fase 1/2/3) staged features so that infrastructure
for a feature (e.g. Auth, Workspaces, SignalR, S3 storage) would only be built
when that feature's phase arrived. A more detailed target folder structure was
later provided covering the entire project's eventual shape — including Auth/
Users/Workspaces (multi-tenant), SignalR real-time sync, S3/local file storage,
i18n, and theming — none of which are Fase 1 MVP features by the original list.

Asked explicitly whether to scaffold all of this now or only as each phase
arrives, the decision was to build the foundation for all of it now, even
where a given area will only become feature-complete in a later phase.

## Decision
Phase "1.0" scaffolds the entire backend and frontend structure from the
target tree, with real (not stub) implementations where the area is
self-contained and well-understood (DDD base classes, the CQRS pipeline, JWT
auth, password hashing, Workspace CRUD, local/S3 file storage, Postgres
full-text search), and intentionally thin/stub implementations only where the
real feature logic is genuinely later-phase work (e.g. Markdown import/export
parsing, the real-time conflict-resolution logic behind the SignalR hub,
graph-view layout algorithms). Stubs are marked with a comment pointing at the
phase they belong to.

## Consequences
- Significantly larger "Fase 1.0" than originally scoped — most of the
  project's eventual surface area exists (compiling, wired into DI) before any
  outliner editor UI exists.
- Later phases become mostly "fill in the handler body" work against an
  already-correct shape, rather than "add a new vertical slice from scratch."
- Risk: a large amount of code lands before the first real end-user feature
  (the block editor) is usable. Mitigated by keeping Domain/Application fully
  unit-tested as it's written, and by verifying the full solution builds and
  all tests pass at the end of this phase before moving to Fase 1.1.

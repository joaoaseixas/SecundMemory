# ADR 0003: Explicit mapping methods instead of AutoMapper

## Status
Accepted

## Context
Same root cause as ADR 0002: AutoMapper (same author as MediatR) now carries
the same commercial-licensing risk for new major versions.

## Decision
`Common/Mappings/` holds plain static extension methods
(`public static PageDto ToDto(this Page page)`) instead of an AutoMapper
`Profile`. Each DTO owns its own mapping, next to its definition.

## Consequences
- No reflection-based mapping magic — breakage shows up at compile time, not
  at runtime when a property gets renamed.
- Slightly more boilerplate per DTO; acceptable given the project's size.
- No external license to track for this concern either.

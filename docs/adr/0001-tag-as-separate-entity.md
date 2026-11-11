# ADR 0001: Tag is a separate entity, not unified with Page

## Status
Accepted (supersedes the Tag=Page unification discussed earlier in the project)

## Context
Logseq's real semantics unify tags and pages: `#foo` and `[[foo]]` resolve to the
same page. Earlier in this project we agreed to follow that unification exactly.

When reviewing a more detailed target folder structure for the backend/frontend,
the structure called for `Tags/Tag.cs` and `ITagRepository.cs` as entities
independent from `Pages/Page.cs`. Asked explicitly, the decision was to keep Tag
separate rather than unify it with Page.

## Decision
`Tag` is its own aggregate (`Domain/Tags/Tag.cs`), independent from `Page`. A
`Block` can be associated with zero or more `Tag`s directly (not via a Page
link). `Tag` can still define a schema of expected properties (the "tags act as
classes" idea from the original request), but that schema lives on `Tag`, not
on an overloaded `Page`.

## Consequences
- Diverges from Logseq's actual data model — worth remembering if we ever
  import a real Logseq graph, since `#tag` there is literally a page reference.
- Simpler mental model for this codebase: Page = a document; Tag = a label with
  optional typed properties. No special-casing needed to tell "page used as a
  tag" apart from "normal page."
- `BlockProperty` values can be seeded from a `Tag`'s defined property schema
  when a block is tagged, same intent as the original "tags function as
  classes" requirement.

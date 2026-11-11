# ADR 0005: Keep the custom textarea-based block editor over TipTap/ProseMirror

## Status
Accepted

## Context
The original Fase 1 instructions asked to evaluate TipTap/ProseMirror and
justify the choice before implementing the block editor. Fase 1.0 shipped a
working outliner (textarea per block, Enter/Tab/Shift+Tab, drag-and-drop,
slash menu, link autocomplete) built directly on a plain `<textarea>`,
without that evaluation. This ADR records it now, before Fase 1.1 polish
work continues on the same foundation.

## Evaluation
**TipTap/ProseMirror** would give: a real contenteditable-based rich-text
document model, built-in undo/redo history, robust IME/composition and
cross-browser selection handling, and a clean extension point for future
collaborative editing (Fase 3 — `y-prosemirror` is the standard CRDT bridge).
The cost: a non-trivial new dependency tree, a schema/Node-type layer to
design for this app's block types, and — critically — a different editing
model than what's actually being built. Logseq's own outliner is not a
single rich-text document; it's one editor instance *per block*, each
holding raw Markdown-like text while focused and a rendered inline view
while blurred. Adopting ProseMirror here would mean one ProseMirror instance
per block, which defeats most of ProseMirror's value (its strength is
modeling one coherent document, not many independent micro-documents) while
still paying its full integration cost.

**Custom textarea-per-block** matches Logseq's actual architecture: a plain
`<textarea>` while editing, a sanitized inline-rendered view
(`MarkdownParserService.renderInline`) while blurred, for `[[links]]`,
`((embeds))`, `**bold**`, `*italic*`. This was already in place but unused;
Fase 1.1 wires the view/edit toggle. No new dependency, same principle
applied earlier to i18n (custom `TranslationService` over `@ngx-translate`
mid-build) — avoid adding dependencies for a problem the existing stack
already solves adequately.

## Decision
Keep the custom textarea + inline-render-on-blur editor for Fase 1.1 and for
the rest of Fase 1/2. Revisit ProseMirror specifically if/when Fase 3's
real-time collaborative editing needs operational-transform/CRDT text
merging that a plain textarea genuinely cannot provide — at that point the
tradeoff (one ProseMirror doc per block, accepting the integration cost) is
worth re-weighing against piping SignalR deltas through a simpler
last-write-wins or block-level lock model.

## Consequences
- No new frontend dependency; editor stays simple to reason about and test.
- Rich formatting stays limited to the hand-rolled inline parser's subset
  (links, embeds, bold, italic) — no tables, no inline code spans, no
  nested marks, until/unless a later phase decides those are worth a parser
  rewrite.
- IME composition and complex selection edge cases are handled by the
  browser's native `<textarea>` behavior, which is more battle-tested here
  than a hand-rolled contenteditable would be, but less customizable than
  ProseMirror's selection model.
- The IME/undo/redo-history question is deferred, not solved; standard
  browser textarea undo (Ctrl+Z) is what's available today, not Logseq-style
  structured undo across the whole page.

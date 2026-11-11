import { Page } from '../models/page.model';

/** Currently a 1:1 passthrough (wire shape already matches the client model) — kept as its own module so a future divergence doesn't mean rewriting call sites. */
export function toDisplayTitle(page: Page): string {
  return page.namespace ? page.title.slice(page.namespace.length + 1) : page.title;
}

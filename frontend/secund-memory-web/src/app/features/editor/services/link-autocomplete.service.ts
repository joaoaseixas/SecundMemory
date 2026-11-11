import { Injectable, inject } from '@angular/core';
import { Page } from '../../pages/models/page.model';
import { PagesStore } from '../../pages/data-access/pages.store';

export interface LinkTrigger {
  query: string;
  /** Index of the opening `[[` — replace from here through the cursor when a suggestion is picked. */
  startIndex: number;
}

/** Pure detection logic + suggestions sourced from whatever pages are already loaded (PagesStore), not a dedicated search round-trip. */
@Injectable({ providedIn: 'root' })
export class LinkAutocompleteService {
  private readonly pagesStore = inject(PagesStore);

  detectTrigger(text: string, cursorIndex: number): LinkTrigger | null {
    const beforeCursor = text.slice(0, cursorIndex);
    const openIndex = beforeCursor.lastIndexOf('[[');
    if (openIndex === -1) return null;

    const sinceOpen = beforeCursor.slice(openIndex + 2);
    if (sinceOpen.includes(']]') || sinceOpen.includes('\n')) return null;

    return { query: sinceOpen, startIndex: openIndex };
  }

  suggestionsFor(query: string): Page[] {
    const normalized = query.trim().toLowerCase();
    return this.pagesStore.pages().filter((p) => p.title.toLowerCase().includes(normalized));
  }

  applySuggestion(text: string, trigger: LinkTrigger, cursorIndex: number, pageTitle: string): { text: string; cursorIndex: number } {
    const before = text.slice(0, trigger.startIndex);
    const after = text.slice(cursorIndex);
    const inserted = `[[${pageTitle}]]`;

    return { text: before + inserted + after, cursorIndex: (before + inserted).length };
  }
}

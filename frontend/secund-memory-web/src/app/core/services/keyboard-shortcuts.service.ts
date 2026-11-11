import { Injectable } from '@angular/core';

export interface ShortcutBinding {
  key: string;
  ctrl?: boolean;
  shift?: boolean;
  handler: () => void;
}

/** Global shortcuts (Ctrl+K for the command palette, etc.) — per-component shortcuts belong in that component instead. */
@Injectable({ providedIn: 'root' })
export class KeyboardShortcutsService {
  private readonly bindings: ShortcutBinding[] = [];

  constructor() {
    window.addEventListener('keydown', (event) => this.handleKeydown(event));
  }

  register(binding: ShortcutBinding): () => void {
    this.bindings.push(binding);
    return () => {
      const index = this.bindings.indexOf(binding);
      if (index >= 0) this.bindings.splice(index, 1);
    };
  }

  private handleKeydown(event: KeyboardEvent): void {
    const match = this.bindings.find(
      (b) =>
        b.key.toLowerCase() === event.key.toLowerCase() &&
        !!b.ctrl === (event.ctrlKey || event.metaKey) &&
        !!b.shift === event.shiftKey,
    );

    if (match) {
      event.preventDefault();
      match.handler();
    }
  }
}

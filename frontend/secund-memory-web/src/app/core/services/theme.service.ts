import { Injectable, effect, signal } from '@angular/core';
import { StorageService } from './storage.service';

export type Theme = 'light' | 'dark';

const STORAGE_KEY = 'theme';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  readonly theme = signal<Theme>('light');

  constructor(private readonly storage: StorageService) {
    const stored = this.storage.get<Theme>(STORAGE_KEY);
    this.theme.set(stored ?? (this.prefersDark() ? 'dark' : 'light'));

    effect(() => {
      document.documentElement.setAttribute('data-theme', this.theme());
      this.storage.set(STORAGE_KEY, this.theme());
    });
  }

  toggle(): void {
    this.theme.set(this.theme() === 'light' ? 'dark' : 'light');
  }

  /** window.matchMedia isn't guaranteed in every environment (test runners, SSR) — default to light rather than throw. */
  private prefersDark(): boolean {
    return typeof window.matchMedia === 'function' && window.matchMedia('(prefers-color-scheme: dark)').matches;
  }
}

import { Injectable, computed, inject, signal } from '@angular/core';
import { StoredSession, TokenStorage } from './token-storage';

@Injectable({ providedIn: 'root' })
export class AuthStore {
  private readonly tokenStorage = inject(TokenStorage);

  readonly session = signal<StoredSession | null>(this.tokenStorage.get());
  readonly isAuthenticated = computed(() => this.session() !== null);
  readonly workspaceId = computed(() => this.session()?.workspaceId ?? null);

  setSession(session: StoredSession): void {
    this.tokenStorage.set(session);
    this.session.set(session);
  }

  clearSession(): void {
    this.tokenStorage.clear();
    this.session.set(null);
  }
}

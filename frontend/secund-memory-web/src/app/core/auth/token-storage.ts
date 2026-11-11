import { Injectable, inject } from '@angular/core';
import { StorageService } from '../services/storage.service';

export interface StoredSession {
  accessToken: string;
  expiresAtUtc: string;
  userId: string;
  workspaceId: string;
}

const STORAGE_KEY = 'session';

@Injectable({ providedIn: 'root' })
export class TokenStorage {
  private readonly storage = inject(StorageService);

  get(): StoredSession | null {
    return this.storage.get<StoredSession>(STORAGE_KEY);
  }

  set(session: StoredSession): void {
    this.storage.set(STORAGE_KEY, session);
  }

  clear(): void {
    this.storage.remove(STORAGE_KEY);
  }
}

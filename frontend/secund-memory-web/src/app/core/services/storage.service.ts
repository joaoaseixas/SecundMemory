import { Injectable } from '@angular/core';

/** Thin wrapper so nothing else in the app touches window.localStorage directly. */
@Injectable({ providedIn: 'root' })
export class StorageService {
  get<T>(key: string): T | null {
    const raw = localStorage.getItem(key);
    if (raw === null) return null;

    try {
      return JSON.parse(raw) as T;
    } catch {
      return null;
    }
  }

  set<T>(key: string, value: T): void {
    localStorage.setItem(key, JSON.stringify(value));
  }

  remove(key: string): void {
    localStorage.removeItem(key);
  }
}

import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { StorageService } from './storage.service';

export type SupportedLocale = 'en' | 'pt-BR';

const STORAGE_KEY = 'locale';
const DEFAULT_LOCALE: SupportedLocale = 'en';

/**
 * Deliberately not @ngx-translate — a flat JSON dictionary with dot-path
 * lookup covers everything Fase 1 needs, with no new npm dependency.
 */
@Injectable({ providedIn: 'root' })
export class TranslationService {
  private readonly http = inject(HttpClient);
  private readonly storage = inject(StorageService);

  private readonly dictionary = signal<Record<string, unknown>>({});
  readonly locale = signal<SupportedLocale>(this.storage.get<SupportedLocale>(STORAGE_KEY) ?? DEFAULT_LOCALE);

  async init(): Promise<void> {
    const data = await firstValueFrom(this.http.get<Record<string, unknown>>(`assets/i18n/${this.locale()}.json`));
    this.dictionary.set(data);
  }

  async setLocale(locale: SupportedLocale): Promise<void> {
    this.locale.set(locale);
    this.storage.set(STORAGE_KEY, locale);
    await this.init();
  }

  translate(key: string): string {
    const value = key.split('.').reduce<unknown>((node, segment) => {
      return typeof node === 'object' && node !== null ? (node as Record<string, unknown>)[segment] : undefined;
    }, this.dictionary());

    return typeof value === 'string' ? value : key;
  }
}

import { Injectable, inject, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { Backlink, Page } from '../models/page.model';
import { PagesApi } from './pages.api';

@Injectable({ providedIn: 'root' })
export class PagesStore {
  private readonly api = inject(PagesApi);

  readonly pages = signal<Page[]>([]);
  readonly currentPage = signal<Page | null>(null);
  readonly backlinks = signal<Backlink[]>([]);
  readonly isLoading = signal(false);

  loadList(): void {
    this.isLoading.set(true);
    this.api.list().subscribe({
      next: (result) => {
        this.pages.set(result.items);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false),
    });
  }

  loadPage(id: string): void {
    this.isLoading.set(true);
    this.api.getById(id).subscribe({
      next: (page) => {
        this.currentPage.set(page);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false),
    });

    this.api.getBacklinks(id).subscribe((backlinks) => this.backlinks.set(backlinks));
  }

  create(title: string): Observable<Page> {
    return this.api.create(title).pipe(tap((page) => this.pages.update((pages) => [...pages, page])));
  }

  rename(id: string, newTitle: string): Observable<Page> {
    return this.api.rename(id, newTitle).pipe(
      tap((updated) => {
        if (this.currentPage()?.id === id) this.currentPage.set(updated);
        this.pages.update((pages) => pages.map((p) => (p.id === id ? updated : p)));
      }),
    );
  }
}

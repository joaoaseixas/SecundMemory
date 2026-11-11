import { Component, inject, signal } from '@angular/core';
import { Subject, debounceTime, switchMap } from 'rxjs';
import { SearchBoxComponent } from '../../components/search-box/search-box.component';
import { SearchResultItemComponent } from '../../components/search-result-item/search-result-item.component';
import { EmptyStateComponent } from '../../../../shared/ui/empty-state/empty-state.component';
import { SearchApi, SearchResult } from '../../data-access/search.api';

const SEARCH_DEBOUNCE_MS = 300;

@Component({
  selector: 'app-search-results',
  standalone: true,
  imports: [SearchBoxComponent, SearchResultItemComponent, EmptyStateComponent],
  templateUrl: './search-results.component.html',
  styleUrl: './search-results.component.scss',
})
export class SearchResultsComponent {
  private readonly searchApi = inject(SearchApi);
  private readonly query$ = new Subject<string>();

  protected readonly query = signal('');
  protected readonly results = signal<SearchResult[]>([]);

  constructor() {
    this.query$.pipe(debounceTime(SEARCH_DEBOUNCE_MS), switchMap((q) => this.searchApi.search(q))).subscribe((results) =>
      this.results.set(results),
    );
  }

  onQueryChange(query: string): void {
    this.query.set(query);
    this.results.set([]);
    if (query.trim()) this.query$.next(query);
  }
}

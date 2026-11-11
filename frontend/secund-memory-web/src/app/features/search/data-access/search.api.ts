import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClientService } from '../../../core/http/api-client.service';
import { ApiEndpoints } from '../../../core/http/api-endpoints';

export interface SearchResult {
  id: string;
  kind: 'Page' | 'Block';
  pageId: string;
  title: string;
  snippet: string;
  rank: number;
}

@Injectable({ providedIn: 'root' })
export class SearchApi {
  private readonly api = inject(ApiClientService);

  search(query: string): Observable<SearchResult[]> {
    return this.api.get<SearchResult[]>(ApiEndpoints.search, { q: query });
  }
}

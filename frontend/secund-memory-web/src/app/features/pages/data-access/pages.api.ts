import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClientService } from '../../../core/http/api-client.service';
import { ApiEndpoints } from '../../../core/http/api-endpoints';
import { PagedResult } from '../../../shared/models/paged-result.model';
import { Backlink, Page } from '../models/page.model';

@Injectable({ providedIn: 'root' })
export class PagesApi {
  private readonly api = inject(ApiClientService);

  list(page = 1, pageSize = 50): Observable<PagedResult<Page>> {
    return this.api.get<PagedResult<Page>>(ApiEndpoints.pages.base, { page, pageSize });
  }

  getById(id: string): Observable<Page> {
    return this.api.get<Page>(ApiEndpoints.pages.byId(id));
  }

  getBacklinks(id: string): Observable<Backlink[]> {
    return this.api.get<Backlink[]>(ApiEndpoints.pages.backlinks(id));
  }

  create(title: string, isJournal = false, journalDate: string | null = null): Observable<Page> {
    return this.api.post<Page>(ApiEndpoints.pages.base, { title, isJournal, journalDate });
  }

  rename(id: string, newTitle: string): Observable<Page> {
    return this.api.put<Page>(ApiEndpoints.pages.title(id), { newTitle });
  }

  delete(id: string): Observable<void> {
    return this.api.delete<void>(ApiEndpoints.pages.byId(id));
  }
}

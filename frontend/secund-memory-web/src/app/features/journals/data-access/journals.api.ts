import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClientService } from '../../../core/http/api-client.service';
import { ApiEndpoints } from '../../../core/http/api-endpoints';

export interface Journal {
  id: string;
  workspaceId: string;
  pageId: string;
  pageTitle: string;
  date: string;
}

@Injectable({ providedIn: 'root' })
export class JournalsApi {
  private readonly api = inject(ApiClientService);

  openToday(): Observable<Journal> {
    return this.api.post<Journal>(ApiEndpoints.journals.today, {});
  }

  /** Get-or-create for any date — never a dead end, unlike the read-only getByDate below. */
  openByDate(date: string): Observable<Journal> {
    return this.api.post<Journal>(ApiEndpoints.journals.byDate(date), {});
  }

  getByDate(date: string): Observable<Journal> {
    return this.api.get<Journal>(ApiEndpoints.journals.byDate(date));
  }
}

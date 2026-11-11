import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClientService } from '../../../core/http/api-client.service';
import { ApiEndpoints } from '../../../core/http/api-endpoints';

export interface Tag {
  id: string;
  workspaceId: string;
  name: string;
  propertyTemplates: { key: string; type: string; defaultValue: string | null; allowMultiple: boolean }[];
}

@Injectable({ providedIn: 'root' })
export class TagsApi {
  private readonly api = inject(ApiClientService);

  list(): Observable<Tag[]> {
    return this.api.get<Tag[]>(ApiEndpoints.tags.base);
  }

  create(name: string): Observable<Tag> {
    return this.api.post<Tag>(ApiEndpoints.tags.base, { name, propertyTemplates: [] });
  }
}

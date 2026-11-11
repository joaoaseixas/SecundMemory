import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClientService } from '../../core/http/api-client.service';
import { ApiEndpoints } from '../../core/http/api-endpoints';

export interface Attachment {
  id: string;
  workspaceId: string;
  blockId: string | null;
  fileName: string;
  contentType: string;
  sizeBytes: number;
}

@Injectable({ providedIn: 'root' })
export class AttachmentsApi {
  private readonly api = inject(ApiClientService);

  list(blockId?: string): Observable<Attachment[]> {
    return this.api.get<Attachment[]>(ApiEndpoints.attachments, blockId ? { blockId } : undefined);
  }
}

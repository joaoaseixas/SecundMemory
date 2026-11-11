import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClientService } from '../../../core/http/api-client.service';
import { ApiEndpoints } from '../../../core/http/api-endpoints';

export interface GraphNode {
  pageId: string;
  title: string;
}

export interface GraphEdge {
  sourcePageId: string;
  targetPageId: string;
}

export interface GraphData {
  nodes: GraphNode[];
  edges: GraphEdge[];
}

@Injectable({ providedIn: 'root' })
export class GraphApi {
  private readonly api = inject(ApiClientService);

  get(): Observable<GraphData> {
    return this.api.get<GraphData>(ApiEndpoints.graph);
  }
}

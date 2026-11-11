import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClientService } from '../../../core/http/api-client.service';
import { ApiEndpoints } from '../../../core/http/api-endpoints';
import { Block, BlockTreeNode, BlockType } from '../models/block.model';

@Injectable({ providedIn: 'root' })
export class BlocksApi {
  private readonly api = inject(ApiClientService);

  getTree(pageId: string): Observable<BlockTreeNode[]> {
    return this.api.get<BlockTreeNode[]>(ApiEndpoints.blocks.treeForPage(pageId));
  }

  create(pageId: string, parentBlockId: string | null, content: string, type: BlockType = 'Text'): Observable<Block> {
    return this.api.post<Block>(ApiEndpoints.blocks.base, { pageId, parentBlockId, content, type });
  }

  updateContent(blockId: string, content: string): Observable<Block> {
    return this.api.put<Block>(ApiEndpoints.blocks.content(blockId), { content });
  }

  move(blockId: string, newParentBlockId: string | null, newOrder: number): Observable<Block> {
    return this.api.put<Block>(ApiEndpoints.blocks.move(blockId), { newParentBlockId, newOrder });
  }

  toggleCollapse(blockId: string): Observable<Block> {
    return this.api.put<Block>(ApiEndpoints.blocks.collapse(blockId), {});
  }

  changeType(blockId: string, type: BlockType): Observable<Block> {
    return this.api.put<Block>(ApiEndpoints.blocks.type(blockId), { type });
  }

  delete(blockId: string): Observable<void> {
    return this.api.delete<void>(ApiEndpoints.blocks.byId(blockId));
  }
}

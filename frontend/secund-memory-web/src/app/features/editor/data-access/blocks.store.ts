import { Injectable, inject, signal } from '@angular/core';
import { Block, BlockTreeNode, BlockType } from '../models/block.model';
import { BlocksApi } from './blocks.api';

/**
 * Content/collapse/type updates patch the local tree in place rather than
 * reloading from the server — reloading after every debounced keystroke
 * would blow away whatever block currently has focus. Structural changes
 * (create/move/delete) DO reload, since recomputing tree shape locally
 * would just duplicate GetBlockTreeHandler's logic for little benefit.
 */
@Injectable({ providedIn: 'root' })
export class BlocksStore {
  private readonly api = inject(BlocksApi);
  private currentPageId: string | null = null;

  readonly tree = signal<BlockTreeNode[]>([]);
  readonly isLoading = signal(false);

  /**
   * Structural ops (create/move/delete) reload the whole tree, which tears
   * down and recreates the affected block-editor component instances —
   * losing DOM focus. Set to the block that should regain focus once the
   * reloaded tree has rendered; BlockEditorComponent claims and clears it.
   */
  readonly pendingFocusId = signal<string | null>(null);

  loadTree(pageId: string): void {
    this.currentPageId = pageId;
    this.isLoading.set(true);

    this.api.getTree(pageId).subscribe({
      next: (tree) => {
        this.tree.set(tree);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false),
    });
  }

  createBlock(parentBlockId: string | null, content = '', type: BlockType = 'Text'): void {
    if (!this.currentPageId) return;
    this.api.create(this.currentPageId, parentBlockId, content, type).subscribe((created) => {
      this.pendingFocusId.set(created.id);
      this.reload();
    });
  }

  updateContent(blockId: string, content: string): void {
    this.api.updateContent(blockId, content).subscribe((updated) => this.patchNode(blockId, () => updated));
  }

  toggleCollapse(blockId: string): void {
    this.api.toggleCollapse(blockId).subscribe((updated) => this.patchNode(blockId, () => updated));
  }

  changeType(blockId: string, type: BlockType): void {
    this.api.changeType(blockId, type).subscribe((updated) => this.patchNode(blockId, () => updated));
  }

  move(blockId: string, newParentBlockId: string | null, newOrder: number): void {
    this.api.move(blockId, newParentBlockId, newOrder).subscribe(() => {
      this.pendingFocusId.set(blockId);
      this.reload();
    });
  }

  deleteBlock(blockId: string, focusAfterId: string | null = null): void {
    this.api.delete(blockId).subscribe(() => {
      this.pendingFocusId.set(focusAfterId);
      this.reload();
    });
  }

  clearPendingFocus(): void {
    this.pendingFocusId.set(null);
  }

  private reload(): void {
    if (this.currentPageId) this.loadTree(this.currentPageId);
  }

  private patchNode(blockId: string, updater: (block: Block) => Block): void {
    const patch = (nodes: BlockTreeNode[]): BlockTreeNode[] =>
      nodes.map((node) =>
        node.block.id === blockId
          ? { ...node, block: updater(node.block) }
          : { ...node, children: patch(node.children) },
      );

    this.tree.update(patch);
  }
}

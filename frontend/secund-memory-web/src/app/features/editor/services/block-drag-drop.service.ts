import { Injectable, inject } from '@angular/core';
import { computeOrderBetween } from '../../../shared/utils/tree.utils';
import { BlockTreeNode } from '../models/block.model';
import { BlocksStore } from '../data-access/blocks.store';

/**
 * Plain HTML5 drag-and-drop (draggable + dragstart/drop events) — no
 * @angular/cdk dependency for what's otherwise a single dragged-block-id
 * and a fractional-order computation.
 */
@Injectable({ providedIn: 'root' })
export class BlockDragDropService {
  private readonly store = inject(BlocksStore);
  private draggedBlockId: string | null = null;

  onDragStart(blockId: string): void {
    this.draggedBlockId = blockId;
  }

  /** Dropped relative to targetNode, among the given siblings, under parentId. */
  onDrop(parentId: string | null, targetNode: BlockTreeNode, position: 'before' | 'after', siblings: BlockTreeNode[]): void {
    if (!this.draggedBlockId || this.draggedBlockId === targetNode.block.id) return;

    const index = siblings.findIndex((n) => n.block.id === targetNode.block.id);
    const before = position === 'before' ? siblings[index - 1]?.block.order ?? null : targetNode.block.order;
    const after = position === 'before' ? targetNode.block.order : (siblings[index + 1]?.block.order ?? null);

    this.store.move(this.draggedBlockId, parentId, computeOrderBetween(before, after));
    this.draggedBlockId = null;
  }
}

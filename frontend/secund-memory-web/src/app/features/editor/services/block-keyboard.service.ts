import { Injectable, inject } from '@angular/core';
import { BlockTreeNode } from '../models/block.model';
import { BlocksStore } from '../data-access/blocks.store';

interface FlatEntry {
  node: BlockTreeNode;
  parent: BlockTreeNode | null;
}

/** Enter/Tab/Shift+Tab/Backspace — the actual outliner behavior (Fase 1 item #1). */
@Injectable({ providedIn: 'root' })
export class BlockKeyboardService {
  private readonly store = inject(BlocksStore);

  /** New sibling after the current block, same parent. */
  handleEnter(blockId: string): void {
    const entry = this.flatten().find((e) => e.node.block.id === blockId);
    if (!entry) return;

    this.store.createBlock(entry.parent?.block.id ?? null, '');
  }

  /** Indent: becomes the last child of its previous sibling. No-op if it's already the first child. */
  handleTab(blockId: string): void {
    const flat = this.flatten();
    const entry = flat.find((e) => e.node.block.id === blockId);
    if (!entry) return;

    const siblings = entry.parent ? entry.parent.children : this.store.tree();
    const index = siblings.findIndex((n) => n.block.id === blockId);
    if (index <= 0) return;

    const previousSibling = siblings[index - 1];
    const newOrder = this.nextOrderAfter(previousSibling.children);
    this.store.move(blockId, previousSibling.block.id, newOrder);
  }

  /** Outdent: becomes a sibling right after its current parent. No-op if already top-level. */
  handleShiftTab(blockId: string): void {
    const flat = this.flatten();
    const entry = flat.find((e) => e.node.block.id === blockId);
    if (!entry?.parent) return;

    const grandparent = flat.find((e) => e.node.block.id === entry.parent!.block.id)?.parent ?? null;
    const newSiblings = grandparent ? grandparent.children : this.store.tree();
    const newOrder = this.nextOrderAfter(newSiblings);

    this.store.move(blockId, grandparent?.block.id ?? null, newOrder);
  }

  /** Backspace on an empty, childless block removes it instead of doing nothing. */
  handleBackspaceOnEmptyBlock(node: BlockTreeNode): void {
    if (node.block.content.length === 0 && node.children.length === 0) {
      this.store.deleteBlock(node.block.id);
    }
  }

  private flatten(nodes: BlockTreeNode[] = this.store.tree(), parent: BlockTreeNode | null = null): FlatEntry[] {
    return nodes.flatMap((node) => [{ node, parent }, ...this.flatten(node.children, node)]);
  }

  private nextOrderAfter(siblings: BlockTreeNode[]): number {
    return siblings.length === 0 ? 1 : Math.max(...siblings.map((n) => n.block.order)) + 1;
  }
}

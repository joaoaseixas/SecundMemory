import { Component, forwardRef, inject, input } from '@angular/core';
import { BlockBulletComponent } from '../block-bullet/block-bullet.component';
import { BlockEditorComponent } from '../block-editor/block-editor.component';
import { BlockTreeNode } from '../../models/block.model';
import { BlocksStore } from '../../data-access/blocks.store';
import { BlockDragDropService } from '../../services/block-drag-drop.service';

@Component({
  selector: 'app-block-item',
  standalone: true,
  // forwardRef: the standard way a standalone component references itself
  // (needed here for recursive rendering — a block's children are blocks).
  imports: [BlockBulletComponent, BlockEditorComponent, forwardRef(() => BlockItemComponent)],
  templateUrl: './block-item.component.html',
  styleUrl: './block-item.component.scss',
})
export class BlockItemComponent {
  readonly node = input.required<BlockTreeNode>();
  readonly parentId = input<string | null>(null);
  readonly siblings = input.required<BlockTreeNode[]>();

  private readonly store = inject(BlocksStore);
  private readonly dragDrop = inject(BlockDragDropService);

  toggleCollapse(): void {
    this.store.toggleCollapse(this.node().block.id);
  }

  onDragStart(): void {
    this.dragDrop.onDragStart(this.node().block.id);
  }

  onDropBefore(event: DragEvent): void {
    event.preventDefault();
    this.dragDrop.onDrop(this.parentId(), this.node(), 'before', this.siblings());
  }

  onDropAfter(event: DragEvent): void {
    event.preventDefault();
    this.dragDrop.onDrop(this.parentId(), this.node(), 'after', this.siblings());
  }
}

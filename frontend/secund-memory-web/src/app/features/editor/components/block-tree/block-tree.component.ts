import { Component, OnInit, inject, input } from '@angular/core';
import { BlockItemComponent } from '../block-item/block-item.component';
import { EmptyStateComponent } from '../../../../shared/ui/empty-state/empty-state.component';
import { BlocksStore } from '../../data-access/blocks.store';

@Component({
  selector: 'app-block-tree',
  standalone: true,
  imports: [BlockItemComponent, EmptyStateComponent],
  templateUrl: './block-tree.component.html',
  styleUrl: './block-tree.component.scss',
})
export class BlockTreeComponent implements OnInit {
  readonly pageId = input.required<string>();

  protected readonly store = inject(BlocksStore);

  ngOnInit(): void {
    this.store.loadTree(this.pageId());
  }

  addFirstBlock(): void {
    this.store.createBlock(null, '');
  }
}

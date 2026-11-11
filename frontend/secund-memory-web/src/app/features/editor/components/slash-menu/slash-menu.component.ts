import { Component, computed, input, output } from '@angular/core';
import { BlockType } from '../../models/block.model';

interface SlashOption {
  label: string;
  type: BlockType;
}

const OPTIONS: SlashOption[] = [
  { label: 'Text', type: 'Text' },
  { label: 'Heading 1', type: 'Heading1' },
  { label: 'Heading 2', type: 'Heading2' },
  { label: 'Heading 3', type: 'Heading3' },
  { label: 'Quote', type: 'Quote' },
  { label: 'Code', type: 'Code' },
];

@Component({
  selector: 'app-slash-menu',
  standalone: true,
  templateUrl: './slash-menu.component.html',
  styleUrl: './slash-menu.component.scss',
})
export class SlashMenuComponent {
  readonly query = input('');
  readonly typeSelected = output<BlockType>();

  protected readonly filtered = computed(() =>
    OPTIONS.filter((option) => option.label.toLowerCase().includes(this.query().toLowerCase())),
  );
}

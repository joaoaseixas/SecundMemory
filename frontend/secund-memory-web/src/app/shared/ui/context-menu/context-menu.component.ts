import { Component, input, output } from '@angular/core';
import { ClickOutsideDirective } from '../../directives/click-outside.directive';

export interface ContextMenuItem {
  label: string;
  action: string;
  danger?: boolean;
}

@Component({
  selector: 'app-context-menu',
  standalone: true,
  imports: [ClickOutsideDirective],
  templateUrl: './context-menu.component.html',
  styleUrl: './context-menu.component.scss',
})
export class ContextMenuComponent {
  readonly items = input.required<ContextMenuItem[]>();
  readonly x = input(0);
  readonly y = input(0);
  readonly itemSelected = output<string>();
  readonly closed = output<void>();
}

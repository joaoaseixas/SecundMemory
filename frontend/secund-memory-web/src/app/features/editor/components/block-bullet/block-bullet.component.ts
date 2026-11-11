import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-block-bullet',
  standalone: true,
  templateUrl: './block-bullet.component.html',
  styleUrl: './block-bullet.component.scss',
})
export class BlockBulletComponent {
  readonly hasChildren = input(false);
  readonly isCollapsed = input(false);
  readonly toggled = output<void>();
}

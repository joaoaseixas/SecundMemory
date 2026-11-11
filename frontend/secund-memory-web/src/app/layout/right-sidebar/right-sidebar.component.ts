import { Component, signal } from '@angular/core';

/** Collapsible host for page-specific panels (backlinks, properties) — those components project in via ng-content. */
@Component({
  selector: 'app-right-sidebar',
  standalone: true,
  templateUrl: './right-sidebar.component.html',
  styleUrl: './right-sidebar.component.scss',
})
export class RightSidebarComponent {
  readonly isOpen = signal(true);

  toggle(): void {
    this.isOpen.update((open) => !open);
  }
}

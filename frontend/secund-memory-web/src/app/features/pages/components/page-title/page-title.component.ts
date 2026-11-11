import { Component, input, output, signal } from '@angular/core';
import { AutoFocusDirective } from '../../../../shared/directives/auto-focus.directive';

@Component({
  selector: 'app-page-title',
  standalone: true,
  imports: [AutoFocusDirective],
  templateUrl: './page-title.component.html',
  styleUrl: './page-title.component.scss',
})
export class PageTitleComponent {
  readonly title = input.required<string>();
  readonly renamed = output<string>();

  readonly isEditing = signal(false);
  readonly draft = signal('');

  startEditing(): void {
    this.draft.set(this.title());
    this.isEditing.set(true);
  }

  commit(): void {
    this.isEditing.set(false);
    const value = this.draft().trim();
    if (value && value !== this.title()) this.renamed.emit(value);
  }

  cancel(): void {
    this.isEditing.set(false);
  }

  onInput(event: Event): void {
    this.draft.set((event.target as HTMLInputElement).value);
  }
}

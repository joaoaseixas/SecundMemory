import { Component, output } from '@angular/core';
import { AutoFocusDirective } from '../../../../shared/directives/auto-focus.directive';

@Component({
  selector: 'app-search-box',
  standalone: true,
  imports: [AutoFocusDirective],
  templateUrl: './search-box.component.html',
  styleUrl: './search-box.component.scss',
})
export class SearchBoxComponent {
  readonly queryChange = output<string>();

  onInput(event: Event): void {
    this.queryChange.emit((event.target as HTMLInputElement).value);
  }
}

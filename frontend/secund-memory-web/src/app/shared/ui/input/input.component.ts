import { Component, input, output } from '@angular/core';

let nextId = 0;

@Component({
  selector: 'app-input',
  standalone: true,
  templateUrl: './input.component.html',
  styleUrl: './input.component.scss',
})
export class InputComponent {
  readonly label = input<string>('');
  readonly type = input<'text' | 'email' | 'password'>('text');
  readonly value = input('');
  readonly placeholder = input('');
  readonly error = input<string | null>(null);
  readonly valueChange = output<string>();

  readonly id = `app-input-${nextId++}`;

  onInput(event: Event): void {
    this.valueChange.emit((event.target as HTMLInputElement).value);
  }
}

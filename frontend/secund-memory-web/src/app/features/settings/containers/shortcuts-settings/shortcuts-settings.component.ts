import { Component } from '@angular/core';

interface ShortcutEntry {
  keys: string;
  description: string;
}

const SHORTCUTS: ShortcutEntry[] = [
  { keys: 'Ctrl+K', description: 'Open the command palette' },
  { keys: 'Enter', description: 'New block' },
  { keys: 'Tab', description: 'Indent block' },
  { keys: 'Shift+Tab', description: 'Outdent block' },
  { keys: '[[', description: 'Link to a page' },
  { keys: '/', description: 'Change block type' },
];

@Component({
  selector: 'app-shortcuts-settings',
  standalone: true,
  templateUrl: './shortcuts-settings.component.html',
  styleUrl: './shortcuts-settings.component.scss',
})
export class ShortcutsSettingsComponent {
  protected readonly shortcuts = SHORTCUTS;
}

import { Component, OnDestroy, OnInit, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { AutoFocusDirective } from '../../directives/auto-focus.directive';
import { KeyboardShortcutsService } from '../../../core/services/keyboard-shortcuts.service';
import { CommandPaletteState } from './command-palette.state';

interface PaletteCommand {
  label: string;
  route: string;
}

const NAV_COMMANDS: PaletteCommand[] = [
  { label: 'Go to Pages', route: '/pages' },
  { label: "Go to Today's Journal", route: '/journal' },
  { label: 'Go to Graph', route: '/graph' },
  { label: 'Go to Search', route: '/search' },
  { label: 'Go to Tags', route: '/tags' },
  { label: 'Go to Settings', route: '/settings' },
];

@Component({
  selector: 'app-command-palette',
  standalone: true,
  imports: [AutoFocusDirective],
  templateUrl: './command-palette.component.html',
  styleUrl: './command-palette.component.scss',
})
export class CommandPaletteComponent implements OnInit, OnDestroy {
  protected readonly state = inject(CommandPaletteState);
  private readonly router = inject(Router);
  private readonly shortcuts = inject(KeyboardShortcutsService);
  private unregister?: () => void;

  protected readonly query = signal('');
  protected readonly filtered = computed(() => {
    const q = this.query().toLowerCase();
    return NAV_COMMANDS.filter((c) => c.label.toLowerCase().includes(q));
  });

  ngOnInit(): void {
    this.unregister = this.shortcuts.register({ key: 'k', ctrl: true, handler: () => this.state.toggle() });
  }

  ngOnDestroy(): void {
    this.unregister?.();
  }

  select(command: PaletteCommand): void {
    this.router.navigateByUrl(command.route);
    this.query.set('');
    this.state.close();
  }

  selectFirst(): void {
    const first = this.filtered()[0];
    if (first) this.select(first);
  }

  onInput(event: Event): void {
    this.query.set((event.target as HTMLInputElement).value);
  }
}

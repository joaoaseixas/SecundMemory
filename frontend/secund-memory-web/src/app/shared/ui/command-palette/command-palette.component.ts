import { Component, OnDestroy, OnInit, computed, effect, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Subject, debounceTime, of, switchMap } from 'rxjs';
import { AutoFocusDirective } from '../../directives/auto-focus.directive';
import { HighlightPipe } from '../../pipes/highlight.pipe';
import { KeyboardShortcutsService } from '../../../core/services/keyboard-shortcuts.service';
import { SearchApi, SearchResult } from '../../../features/search/data-access/search.api';
import { CommandPaletteState } from './command-palette.state';

const SEARCH_DEBOUNCE_MS = 300;
const MIN_SEARCH_QUERY_LENGTH = 2;

interface NavCommand {
  type: 'nav';
  label: string;
  route: string;
}

interface ResultCommand {
  type: 'result';
  result: SearchResult;
}

type PaletteItem = NavCommand | ResultCommand;

const NAV_COMMANDS: Omit<NavCommand, 'type'>[] = [
  { label: 'Go to Pages', route: '/pages' },
  { label: "Go to Today's Journal", route: '/journal' },
  { label: 'Go to Graph', route: '/graph' },
  { label: 'Go to Search', route: '/search' },
  { label: 'Go to Tags', route: '/tags' },
  { label: 'Go to Settings', route: '/settings' },
];

/**
 * Doubles as the app's quick-search entry point: below the static nav
 * commands, it searches pages/blocks live (same SearchApi the dedicated
 * /search page uses) so Ctrl+K reaches actual content, not just navigation.
 */
@Component({
  selector: 'app-command-palette',
  standalone: true,
  imports: [AutoFocusDirective, HighlightPipe],
  templateUrl: './command-palette.component.html',
  styleUrl: './command-palette.component.scss',
})
export class CommandPaletteComponent implements OnInit, OnDestroy {
  protected readonly state = inject(CommandPaletteState);
  private readonly router = inject(Router);
  private readonly shortcuts = inject(KeyboardShortcutsService);
  private readonly searchApi = inject(SearchApi);
  private unregister?: () => void;

  protected readonly query = signal('');
  private readonly searchResults = signal<SearchResult[]>([]);
  private readonly querySubmitted$ = new Subject<string>();

  protected readonly navMatches = computed<NavCommand[]>(() => {
    const q = this.query().toLowerCase();
    return NAV_COMMANDS.filter((c) => c.label.toLowerCase().includes(q)).map((c) => ({ type: 'nav' as const, ...c }));
  });

  protected readonly items = computed<PaletteItem[]>(() => [
    ...this.navMatches(),
    ...this.searchResults().map((result): ResultCommand => ({ type: 'result', result })),
  ]);

  constructor() {
    this.querySubmitted$
      .pipe(
        debounceTime(SEARCH_DEBOUNCE_MS),
        switchMap((q) => (q.trim().length >= MIN_SEARCH_QUERY_LENGTH ? this.searchApi.search(q) : of([]))),
      )
      .subscribe((results) => this.searchResults.set(results));

    effect(() => {
      if (!this.state.isOpen()) {
        this.query.set('');
        this.searchResults.set([]);
      }
    });
  }

  ngOnInit(): void {
    this.unregister = this.shortcuts.register({ key: 'k', ctrl: true, handler: () => this.state.toggle() });
  }

  ngOnDestroy(): void {
    this.unregister?.();
  }

  select(item: PaletteItem): void {
    const route = item.type === 'nav' ? item.route : `/pages/${item.result.pageId}`;
    this.router.navigateByUrl(route);
    this.state.close();
  }

  selectFirst(): void {
    const first = this.items()[0];
    if (first) this.select(first);
  }

  onInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.query.set(value);
    this.querySubmitted$.next(value);
  }
}

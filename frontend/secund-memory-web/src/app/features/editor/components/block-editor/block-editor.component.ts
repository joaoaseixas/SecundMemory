import { AfterViewInit, Component, ElementRef, Injector, OnDestroy, OnInit, afterNextRender, inject, input, signal, viewChild } from '@angular/core';
import { Router } from '@angular/router';
import { Subject, Subscription, debounceTime } from 'rxjs';
import { SlashMenuComponent } from '../slash-menu/slash-menu.component';
import { MarkdownParserService } from '../../services/markdown-parser.service';
import { BlockType, BlockTreeNode } from '../../models/block.model';
import { BlocksStore } from '../../data-access/blocks.store';
import { BlockKeyboardService } from '../../services/block-keyboard.service';
import { LinkAutocompleteService, LinkTrigger } from '../../services/link-autocomplete.service';
import { PagesApi } from '../../../pages/data-access/pages.api';
import { Page } from '../../../pages/models/page.model';

const SAVE_DEBOUNCE_MS = 400;

@Component({
  selector: 'app-block-editor',
  standalone: true,
  imports: [SlashMenuComponent],
  templateUrl: './block-editor.component.html',
  styleUrl: './block-editor.component.scss',
})
export class BlockEditorComponent implements OnInit, AfterViewInit, OnDestroy {
  readonly node = input.required<BlockTreeNode>();

  private readonly store = inject(BlocksStore);
  private readonly keyboard = inject(BlockKeyboardService);
  private readonly linkAutocomplete = inject(LinkAutocompleteService);
  private readonly markdownParser = inject(MarkdownParserService);
  private readonly pagesApi = inject(PagesApi);
  private readonly router = inject(Router);
  private readonly injector = inject(Injector);

  private readonly textarea = viewChild<ElementRef<HTMLTextAreaElement>>('textarea');

  protected readonly draft = signal('');
  protected readonly focused = signal(false);
  protected readonly showSlashMenu = signal(false);
  protected readonly slashQuery = signal('');
  protected readonly linkSuggestions = signal<Page[]>([]);

  private linkTrigger: LinkTrigger | null = null;
  private readonly contentChange$ = new Subject<string>();
  private subscription?: Subscription;

  ngOnInit(): void {
    this.draft.set(this.node().block.content);
    this.subscription = this.contentChange$.pipe(debounceTime(SAVE_DEBOUNCE_MS)).subscribe((value) => {
      this.store.updateContent(this.node().block.id, value);
    });
  }

  ngAfterViewInit(): void {
    if (this.store.pendingFocusId() === this.node().block.id) {
      this.store.clearPendingFocus();
      this.enterEditMode();
    }
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
  }

  protected renderedHtml() {
    return this.markdownParser.renderInline(this.draft());
  }

  protected showRendered(): boolean {
    return !this.focused() && this.draft().trim().length > 0;
  }

  onFocus(): void {
    this.focused.set(true);
  }

  onBlur(): void {
    this.focused.set(false);
    this.showSlashMenu.set(false);
    this.linkSuggestions.set([]);
  }

  /** Clicking the rendered view either follows a [[link]] or, anywhere else, enters edit mode. */
  onRenderedClick(event: MouseEvent): void {
    const target = event.target as HTMLElement;
    const pageTitle = target.closest<HTMLElement>('.inline-link')?.dataset['pageTitle'];

    if (pageTitle) {
      this.pagesApi.getByTitle(pageTitle).subscribe((page) => {
        if (page) this.router.navigate(['/pages', page.id]);
      });
      return;
    }

    this.enterEditMode();
  }

  onInput(event: Event): void {
    const textarea = event.target as HTMLTextAreaElement;
    const value = textarea.value;
    this.draft.set(value);
    this.contentChange$.next(value);

    if (value.startsWith('/')) {
      this.showSlashMenu.set(true);
      this.slashQuery.set(value.slice(1));
    } else {
      this.showSlashMenu.set(false);
    }

    this.linkTrigger = this.linkAutocomplete.detectTrigger(value, textarea.selectionStart);
    this.linkSuggestions.set(this.linkTrigger ? this.linkAutocomplete.suggestionsFor(this.linkTrigger.query) : []);
  }

  onKeydown(event: KeyboardEvent): void {
    const textarea = event.target as HTMLTextAreaElement;

    if (event.key === 'Enter' && !event.shiftKey) {
      event.preventDefault();
      this.keyboard.handleEnter(this.node().block.id);
    } else if (event.key === 'Tab') {
      event.preventDefault();
      if (event.shiftKey) this.keyboard.handleShiftTab(this.node().block.id);
      else this.keyboard.handleTab(this.node().block.id);
    } else if (event.key === 'Backspace' && this.draft().length === 0) {
      event.preventDefault();
      this.keyboard.handleBackspaceOnEmptyBlock(this.node());
    } else if (event.key === 'Escape' && (this.showSlashMenu() || this.linkSuggestions().length > 0)) {
      event.preventDefault();
      this.showSlashMenu.set(false);
      this.linkSuggestions.set([]);
    } else if (event.key === 'ArrowUp' && this.isCursorOnFirstLine(textarea)) {
      this.focusSibling(this.keyboard.previousBlockId(this.node().block.id));
    } else if (event.key === 'ArrowDown' && this.isCursorOnLastLine(textarea)) {
      this.focusSibling(this.keyboard.nextBlockId(this.node().block.id));
    }
  }

  selectBlockType(type: BlockType): void {
    this.store.changeType(this.node().block.id, type);
    this.setDraft('');
    this.showSlashMenu.set(false);
  }

  selectLinkSuggestion(pageTitle: string, textarea: HTMLTextAreaElement): void {
    if (!this.linkTrigger) return;

    const result = this.linkAutocomplete.applySuggestion(this.draft(), this.linkTrigger, textarea.selectionStart, pageTitle);
    this.setDraft(result.text);
    this.linkSuggestions.set([]);
  }

  private setDraft(value: string): void {
    this.draft.set(value);
    this.contentChange$.next(value);
  }

  private isCursorOnFirstLine(textarea: HTMLTextAreaElement): boolean {
    return !textarea.value.slice(0, textarea.selectionStart).includes('\n');
  }

  private isCursorOnLastLine(textarea: HTMLTextAreaElement): boolean {
    return !textarea.value.slice(textarea.selectionEnd).includes('\n');
  }

  /** Blocks outside the current one aren't this component's view, so focus is resolved via a DOM lookup rather than a signal. */
  private focusSibling(blockId: string | null): void {
    if (!blockId) return;
    document.querySelector<HTMLTextAreaElement>(`textarea[data-block-id="${blockId}"]`)?.focus();
  }

  /**
   * Switches to the textarea and focuses it. The textarea is [hidden] while
   * showRendered() is true, and a hidden element can't receive focus — so
   * this flips `focused` first, then waits for that to actually hit the DOM
   * (afterNextRender) before calling .focus().
   */
  private enterEditMode(): void {
    this.focused.set(true);
    afterNextRender(() => this.textarea()?.nativeElement.focus(), { injector: this.injector });
  }
}

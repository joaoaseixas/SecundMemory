import { Component, OnDestroy, OnInit, inject, input, signal } from '@angular/core';
import { Subject, Subscription, debounceTime } from 'rxjs';
import { SlashMenuComponent } from '../slash-menu/slash-menu.component';
import { BlockType, BlockTreeNode } from '../../models/block.model';
import { BlocksStore } from '../../data-access/blocks.store';
import { BlockKeyboardService } from '../../services/block-keyboard.service';
import { LinkAutocompleteService, LinkTrigger } from '../../services/link-autocomplete.service';
import { Page } from '../../../pages/models/page.model';

const SAVE_DEBOUNCE_MS = 400;

@Component({
  selector: 'app-block-editor',
  standalone: true,
  imports: [SlashMenuComponent],
  templateUrl: './block-editor.component.html',
  styleUrl: './block-editor.component.scss',
})
export class BlockEditorComponent implements OnInit, OnDestroy {
  readonly node = input.required<BlockTreeNode>();

  private readonly store = inject(BlocksStore);
  private readonly keyboard = inject(BlockKeyboardService);
  private readonly linkAutocomplete = inject(LinkAutocompleteService);

  protected readonly draft = signal('');
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

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
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
}

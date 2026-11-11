import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BlockTreeComponent } from '../../../editor/components/block-tree/block-tree.component';
import { EmptyStateComponent } from '../../../../shared/ui/empty-state/empty-state.component';
import { JournalCalendarComponent } from '../../components/journal-calendar/journal-calendar.component';
import { JournalsApi } from '../../data-access/journals.api';
import { formatJournalDateLong, shiftJournalDate, toJournalDateString } from '../../../../shared/utils/date.utils';

/**
 * One view for "today" and "any other date" (Fase 1.2) — Logseq's journal is
 * a single navigable timeline, not two disconnected screens. The route param
 * is optional: its absence means "today," resolved server-side (openToday)
 * rather than computed from the client clock, so the server's notion of
 * "today" is always the source of truth.
 */
@Component({
  selector: 'app-journal',
  standalone: true,
  imports: [BlockTreeComponent, EmptyStateComponent, JournalCalendarComponent],
  templateUrl: './journal.component.html',
  styleUrl: './journal.component.scss',
})
export class JournalComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly journalsApi = inject(JournalsApi);

  protected readonly pageId = signal<string | null>(null);
  protected readonly date = signal('');
  protected readonly isLoading = signal(false);
  protected readonly hasError = signal(false);

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      const requested = params.get('date');
      if (requested) this.open(this.journalsApi.openByDate(requested));
      else this.open(this.journalsApi.openToday());
    });
  }

  protected heading(): string {
    return this.date() ? formatJournalDateLong(this.date()) : '';
  }

  protected isToday(): boolean {
    return this.date() === toJournalDateString(new Date());
  }

  goToPreviousDay(): void {
    this.router.navigate(['/journal', shiftJournalDate(this.date(), -1)]);
  }

  goToNextDay(): void {
    this.router.navigate(['/journal', shiftJournalDate(this.date(), 1)]);
  }

  goToToday(): void {
    this.router.navigate(['/journal']);
  }

  private open(request: ReturnType<JournalsApi['openToday']>): void {
    this.isLoading.set(true);
    this.hasError.set(false);
    request.subscribe({
      next: (journal) => {
        this.pageId.set(journal.pageId);
        this.date.set(journal.date);
        this.isLoading.set(false);
      },
      error: () => {
        this.hasError.set(true);
        this.isLoading.set(false);
      },
    });
  }
}

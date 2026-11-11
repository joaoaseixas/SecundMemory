import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BlockTreeComponent } from '../../../editor/components/block-tree/block-tree.component';
import { EmptyStateComponent } from '../../../../shared/ui/empty-state/empty-state.component';
import { JournalCalendarComponent } from '../../components/journal-calendar/journal-calendar.component';
import { JournalsApi } from '../../data-access/journals.api';

@Component({
  selector: 'app-journal-history',
  standalone: true,
  imports: [BlockTreeComponent, EmptyStateComponent, JournalCalendarComponent],
  templateUrl: './journal-history.component.html',
  styleUrl: './journal-history.component.scss',
})
export class JournalHistoryComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly journalsApi = inject(JournalsApi);

  protected readonly pageId = signal<string | null>(null);
  protected readonly notFound = signal(false);

  ngOnInit(): void {
    const date = this.route.snapshot.paramMap.get('date');
    if (!date) return;

    this.journalsApi.getByDate(date).subscribe({
      next: (journal) => this.pageId.set(journal.pageId),
      error: () => this.notFound.set(true),
    });
  }
}

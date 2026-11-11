import { Component, OnInit, inject, signal } from '@angular/core';
import { BlockTreeComponent } from '../../../editor/components/block-tree/block-tree.component';
import { JournalsApi } from '../../data-access/journals.api';

@Component({
  selector: 'app-journal-today',
  standalone: true,
  imports: [BlockTreeComponent],
  templateUrl: './journal-today.component.html',
  styleUrl: './journal-today.component.scss',
})
export class JournalTodayComponent implements OnInit {
  private readonly journalsApi = inject(JournalsApi);
  protected readonly pageId = signal<string | null>(null);
  protected readonly title = signal('');

  ngOnInit(): void {
    this.journalsApi.openToday().subscribe((journal) => {
      this.pageId.set(journal.pageId);
      this.title.set(journal.pageTitle);
    });
  }
}

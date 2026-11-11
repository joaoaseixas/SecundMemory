import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { toJournalDateString } from '../../../../shared/utils/date.utils';

interface DayEntry {
  date: string;
  label: string;
}

const DAYS_BACK = 14;

/** A date list, not a month grid — a full calendar widget is more Fase 2 polish than Fase 1 needs. */
@Component({
  selector: 'app-journal-calendar',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './journal-calendar.component.html',
  styleUrl: './journal-calendar.component.scss',
})
export class JournalCalendarComponent {
  /** The date currently open in the main journal view, for highlighting the matching entry. */
  readonly activeDate = input('');

  protected readonly days: DayEntry[] = Array.from({ length: DAYS_BACK }, (_, i) => {
    const date = new Date();
    date.setDate(date.getDate() - i);
    return { date: toJournalDateString(date), label: date.toLocaleDateString(undefined, { weekday: 'short', month: 'short', day: 'numeric' }) };
  });
}

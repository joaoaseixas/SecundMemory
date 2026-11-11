import { Routes } from '@angular/router';
import { JournalTodayComponent } from './containers/journal-today/journal-today.component';
import { JournalHistoryComponent } from './containers/journal-history/journal-history.component';

export const journalsRoutes: Routes = [
  { path: '', component: JournalTodayComponent },
  { path: 'history/:date', component: JournalHistoryComponent },
];

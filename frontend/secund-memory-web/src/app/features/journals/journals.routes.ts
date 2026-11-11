import { Routes } from '@angular/router';
import { JournalComponent } from './containers/journal/journal.component';

export const journalsRoutes: Routes = [
  { path: '', component: JournalComponent },
  { path: ':date', component: JournalComponent },
];

import { Routes } from '@angular/router';
import { PageListComponent } from './containers/page-list/page-list.component';
import { PageViewComponent } from './containers/page-view/page-view.component';

export const pagesRoutes: Routes = [
  { path: '', component: PageListComponent },
  { path: ':id', component: PageViewComponent },
];

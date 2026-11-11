import { Routes } from '@angular/router';
import { authGuard } from './core/auth/guards/auth.guard';
import { AuthLayoutComponent } from './layout/auth-layout/auth-layout.component';
import { MainLayoutComponent } from './layout/main-layout/main-layout.component';

export const routes: Routes = [
  {
    path: '',
    component: AuthLayoutComponent,
    children: [{ path: '', loadChildren: () => import('./features/auth/auth.routes').then((m) => m.authRoutes) }],
  },
  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'journal', pathMatch: 'full' },
      { path: 'journal', loadChildren: () => import('./features/journals/journals.routes').then((m) => m.journalsRoutes) },
      { path: 'pages', loadChildren: () => import('./features/pages/pages.routes').then((m) => m.pagesRoutes) },
      { path: 'graph', loadChildren: () => import('./features/graph/graph.routes').then((m) => m.graphRoutes) },
      { path: 'search', loadChildren: () => import('./features/search/search.routes').then((m) => m.searchRoutes) },
      { path: 'tags', loadChildren: () => import('./features/tags/tags.routes').then((m) => m.tagsRoutes) },
      { path: 'attachments', loadChildren: () => import('./features/attachments/attachments.routes').then((m) => m.attachmentsRoutes) },
      { path: 'import-export', loadChildren: () => import('./features/import-export/import-export.routes').then((m) => m.importExportRoutes) },
      { path: 'settings', loadChildren: () => import('./features/settings/settings.routes').then((m) => m.settingsRoutes) },
    ],
  },
];

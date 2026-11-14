import { Component } from '@angular/core';
import { EmptyStateComponent } from '../../shared/ui/empty-state/empty-state.component';

/** Stub matching backend ImportMarkdownHandler/ExportMarkdownHandler (both throw NotImplementedException) — see docs/adr/0004, Fase 1 item #7. */
@Component({
  selector: 'app-import-export',
  standalone: true,
  imports: [EmptyStateComponent],
  template: `<app-empty-state title="Import and export aren't available yet" description="Markdown import and export will appear here." />`,
})
export class ImportExportComponent {}

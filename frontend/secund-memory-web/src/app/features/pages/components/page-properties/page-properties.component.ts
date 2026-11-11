import { Component, input } from '@angular/core';
import { Page } from '../../models/page.model';

/** Read-only for Fase 1 — typed-property editing (Fase 1.5) will extend this, not replace it. */
@Component({
  selector: 'app-page-properties',
  standalone: true,
  templateUrl: './page-properties.component.html',
  styleUrl: './page-properties.component.scss',
})
export class PagePropertiesComponent {
  readonly page = input.required<Page>();
}

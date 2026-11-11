import { Component, output } from '@angular/core';

/** Stub: filtering (by tag/namespace) only matters once there's an actual visual graph to filter — see graph-canvas. */
@Component({
  selector: 'app-graph-filters',
  standalone: true,
  template: `<p class="graph-filters__note">Filters arrive with the visual graph (Fase 2).</p>`,
  styles: [`.graph-filters__note { color: var(--color-text-muted); font-size: var(--font-size-sm); }`],
})
export class GraphFiltersComponent {
  readonly filterChanged = output<string>();
}

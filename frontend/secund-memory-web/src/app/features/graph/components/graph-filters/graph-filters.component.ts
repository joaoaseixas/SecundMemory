import { Component, output, signal } from '@angular/core';
import { DEFAULT_GRAPH_FILTERS, GraphFilters } from '../../data-access/graph-filter';

@Component({
  selector: 'app-graph-filters',
  standalone: true,
  template: `
    <div class="graph-filters">
      <input
        class="graph-filters__search"
        type="search"
        placeholder="Filter pages by title…"
        [value]="filters().query"
        (input)="setQuery($any($event.target).value)"
      />
      <label class="graph-filters__toggle">
        <input
          type="checkbox"
          [checked]="filters().hideOrphans"
          (change)="setHideOrphans($any($event.target).checked)"
        />
        Hide orphan pages
      </label>
    </div>
  `,
  styles: [
    `
      .graph-filters {
        display: flex;
        flex-wrap: wrap;
        align-items: center;
        gap: var(--spacing-md);
        margin-bottom: var(--spacing-md);
      }
      .graph-filters__search {
        flex: 1 1 220px;
        max-width: 360px;
        padding: var(--spacing-xs) var(--spacing-sm);
      }
      .graph-filters__toggle {
        color: var(--color-text-muted);
        font-size: var(--font-size-sm);
        display: inline-flex;
        gap: var(--spacing-xs);
        align-items: center;
      }
    `,
  ],
})
export class GraphFiltersComponent {
  readonly filterChanged = output<GraphFilters>();
  protected readonly filters = signal<GraphFilters>(DEFAULT_GRAPH_FILTERS);

  protected setQuery(query: string): void {
    this.update({ ...this.filters(), query });
  }

  protected setHideOrphans(hideOrphans: boolean): void {
    this.update({ ...this.filters(), hideOrphans });
  }

  private update(next: GraphFilters): void {
    this.filters.set(next);
    this.filterChanged.emit(next);
  }
}

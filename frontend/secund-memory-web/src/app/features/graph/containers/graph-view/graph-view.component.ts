import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { GraphCanvasComponent } from '../../components/graph-canvas/graph-canvas.component';
import { GraphFiltersComponent } from '../../components/graph-filters/graph-filters.component';
import { DEFAULT_GRAPH_FILTERS, GraphFilters, filterGraph } from '../../data-access/graph-filter';
import { GraphApi, GraphData } from '../../data-access/graph.api';

@Component({
  selector: 'app-graph-view',
  standalone: true,
  imports: [GraphCanvasComponent, GraphFiltersComponent],
  templateUrl: './graph-view.component.html',
  styleUrl: './graph-view.component.scss',
})
export class GraphViewComponent implements OnInit {
  private readonly graphApi = inject(GraphApi);
  private readonly raw = signal<GraphData | null>(null);
  private readonly filters = signal<GraphFilters>(DEFAULT_GRAPH_FILTERS);

  protected readonly data = computed(() => {
    const raw = this.raw();
    return raw ? filterGraph(raw, this.filters()) : null;
  });

  ngOnInit(): void {
    this.graphApi.get().subscribe((data) => this.raw.set(data));
  }

  protected onFilterChanged(filters: GraphFilters): void {
    this.filters.set(filters);
  }
}

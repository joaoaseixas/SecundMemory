import { Component, OnInit, inject, signal } from '@angular/core';
import { GraphCanvasComponent } from '../../components/graph-canvas/graph-canvas.component';
import { GraphFiltersComponent } from '../../components/graph-filters/graph-filters.component';
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
  protected readonly data = signal<GraphData | null>(null);

  ngOnInit(): void {
    this.graphApi.get().subscribe((data) => this.data.set(data));
  }
}

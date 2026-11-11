import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { GraphData } from '../../data-access/graph.api';

/**
 * Textual connection list, not a force-directed canvas — the real visual
 * graph (Cytoscape.js/D3, per the original Fase 2 plan) is item #11, not
 * Fase 1. The data backing it (GetGraphDataQuery) already works end to end;
 * only the rendering is deferred.
 */
@Component({
  selector: 'app-graph-canvas',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './graph-canvas.component.html',
  styleUrl: './graph-canvas.component.scss',
})
export class GraphCanvasComponent {
  readonly data = input.required<GraphData>();

  titleOf(pageId: string): string {
    return this.data().nodes.find((n) => n.pageId === pageId)?.title ?? pageId;
  }
}

import { GraphData } from './graph.api';

export interface GraphFilters {
  query: string;
  hideOrphans: boolean;
}

export const DEFAULT_GRAPH_FILTERS: GraphFilters = { query: '', hideOrphans: false };

/**
 * Client-side graph filtering. A title query keeps matching pages plus their
 * direct neighbours (so you see what a match connects to); orphans (pages with
 * no links in or out) can be hidden. Edges are kept only if both ends survive.
 */
export function filterGraph(data: GraphData, filters: GraphFilters): GraphData {
  const query = filters.query.trim().toLowerCase();
  let keep = new Set(data.nodes.map((n) => n.pageId));

  if (query) {
    const matches = new Set(
      data.nodes.filter((n) => n.title.toLowerCase().includes(query)).map((n) => n.pageId),
    );
    keep = new Set(matches);
    for (const e of data.edges) {
      if (matches.has(e.sourcePageId)) keep.add(e.targetPageId);
      if (matches.has(e.targetPageId)) keep.add(e.sourcePageId);
    }
  }

  let edges = data.edges.filter((e) => keep.has(e.sourcePageId) && keep.has(e.targetPageId));

  if (filters.hideOrphans) {
    const linked = new Set<string>();
    for (const e of edges) {
      linked.add(e.sourcePageId);
      linked.add(e.targetPageId);
    }
    keep = new Set([...keep].filter((id) => linked.has(id)));
    edges = edges.filter((e) => keep.has(e.sourcePageId) && keep.has(e.targetPageId));
  }

  return { nodes: data.nodes.filter((n) => keep.has(n.pageId)), edges };
}

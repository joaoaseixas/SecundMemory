import { describe, expect, it } from 'vitest';
import { filterGraph } from './graph-filter';

const data = {
  nodes: [
    { pageId: 'a', title: 'Angular' },
    { pageId: 'b', title: 'Backend' },
    { pageId: 'c', title: 'Cooking' },
    { pageId: 'd', title: 'Lonely' },
  ],
  edges: [
    { sourcePageId: 'a', targetPageId: 'b' },
    { sourcePageId: 'b', targetPageId: 'c' },
  ],
};

const ids = (g: { nodes: { pageId: string }[] }) => g.nodes.map((n) => n.pageId).sort();

describe('filterGraph', () => {
  it('returns everything with default filters', () => {
    const g = filterGraph(data, { query: '', hideOrphans: false });
    expect(ids(g)).toEqual(['a', 'b', 'c', 'd']);
    expect(g.edges.length).toBe(2);
  });

  it('query keeps matches and their direct neighbours only', () => {
    const g = filterGraph(data, { query: 'angular', hideOrphans: false });
    expect(ids(g)).toEqual(['a', 'b']);
    expect(g.edges).toEqual([{ sourcePageId: 'a', targetPageId: 'b' }]);
  });

  it('hides orphans', () => {
    const g = filterGraph(data, { query: '', hideOrphans: true });
    expect(ids(g)).toEqual(['a', 'b', 'c']);
  });

  it('is case-insensitive and ignores surrounding spaces', () => {
    expect(ids(filterGraph(data, { query: '  LONELY ', hideOrphans: false }))).toEqual(['d']);
  });
});

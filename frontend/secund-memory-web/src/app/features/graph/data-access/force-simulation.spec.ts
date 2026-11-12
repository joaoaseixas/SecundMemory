import { describe, expect, it } from 'vitest';
import { ForceSimulation } from './force-simulation';

const data = {
  nodes: [
    { pageId: 'a', title: 'A' },
    { pageId: 'b', title: 'B' },
    { pageId: 'c', title: 'C' },
  ],
  edges: [
    { sourcePageId: 'a', targetPageId: 'b' },
    { sourcePageId: 'a', targetPageId: 'ghost' },
    { sourcePageId: 'a', targetPageId: 'a' },
  ],
};

describe('ForceSimulation', () => {
  it('ignores dangling and self edges and counts degree', () => {
    const sim = new ForceSimulation(data, 800, 600);
    expect(sim.edges.length).toBe(1);
    expect(sim.nodes.find((n) => n.id === 'a')!.degree).toBe(1);
    expect(sim.nodes.find((n) => n.id === 'c')!.degree).toBe(0);
  });

  it('settles with finite coordinates and keeps nodes apart', () => {
    const sim = new ForceSimulation(data, 800, 600);
    sim.run();
    expect(sim.settled).toBe(true);
    for (const n of sim.nodes) {
      expect(Number.isFinite(n.x)).toBe(true);
      expect(Number.isFinite(n.y)).toBe(true);
    }
    const [a, b] = sim.nodes;
    expect(Math.hypot(a.x - b.x, a.y - b.y)).toBeGreaterThan(10);
  });

  it('handles an empty graph', () => {
    const sim = new ForceSimulation({ nodes: [], edges: [] }, 800, 600);
    sim.run();
    expect(sim.nodes.length).toBe(0);
  });
});

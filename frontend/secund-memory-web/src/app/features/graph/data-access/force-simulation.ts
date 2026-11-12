import { GraphData } from './graph.api';

export interface SimNode {
  id: string;
  title: string;
  x: number;
  y: number;
  vx: number;
  vy: number;
  degree: number;
}

export interface SimEdge {
  source: SimNode;
  target: SimNode;
}

/**
 * Small dependency-free force-directed layout: pairwise repulsion, spring
 * attraction along edges, gentle gravity to the center, cooling via `alpha`.
 */
export class ForceSimulation {
  readonly nodes: SimNode[];
  readonly edges: SimEdge[];
  alpha = 1;

  constructor(
    data: GraphData,
    private width: number,
    private height: number,
  ) {
    const byId = new Map<string, SimNode>();
    const count = Math.max(data.nodes.length, 1);
    this.nodes = data.nodes.map((n, i) => {
      // Deterministic circular seed so layouts are reproducible.
      const angle = (2 * Math.PI * i) / count;
      const radius = Math.min(width, height) / 3;
      const node: SimNode = {
        id: n.pageId,
        title: n.title,
        x: width / 2 + radius * Math.cos(angle),
        y: height / 2 + radius * Math.sin(angle),
        vx: 0,
        vy: 0,
        degree: 0,
      };
      byId.set(n.pageId, node);
      return node;
    });
    this.edges = [];
    for (const e of data.edges) {
      const source = byId.get(e.sourcePageId);
      const target = byId.get(e.targetPageId);
      if (!source || !target || source === target) continue;
      source.degree++;
      target.degree++;
      this.edges.push({ source, target });
    }
  }

  resize(width: number, height: number): void {
    this.width = width;
    this.height = height;
    this.alpha = Math.max(this.alpha, 0.3);
  }

  reheat(): void {
    this.alpha = Math.max(this.alpha, 0.5);
  }

  get settled(): boolean {
    return this.alpha < 0.005;
  }

  step(): void {
    const { nodes, edges } = this;
    const repulsion = 2500;
    const spring = 0.02;
    const restLength = 90;
    const gravity = 0.01;

    for (let i = 0; i < nodes.length; i++) {
      for (let j = i + 1; j < nodes.length; j++) {
        const a = nodes[i];
        const b = nodes[j];
        let dx = b.x - a.x;
        let dy = b.y - a.y;
        let dist2 = dx * dx + dy * dy;
        if (dist2 < 0.01) {
          dx = (i % 2 ? 1 : -1) * 0.1;
          dy = (j % 2 ? 1 : -1) * 0.1;
          dist2 = dx * dx + dy * dy;
        }
        const dist = Math.sqrt(dist2);
        const force = (repulsion / dist2) * this.alpha;
        const fx = (dx / dist) * force;
        const fy = (dy / dist) * force;
        a.vx -= fx;
        a.vy -= fy;
        b.vx += fx;
        b.vy += fy;
      }
    }

    for (const { source, target } of edges) {
      const dx = target.x - source.x;
      const dy = target.y - source.y;
      const dist = Math.sqrt(dx * dx + dy * dy) || 1;
      const force = (dist - restLength) * spring * this.alpha;
      const fx = (dx / dist) * force;
      const fy = (dy / dist) * force;
      source.vx += fx;
      source.vy += fy;
      target.vx -= fx;
      target.vy -= fy;
    }

    for (const n of nodes) {
      n.vx += (this.width / 2 - n.x) * gravity * this.alpha;
      n.vy += (this.height / 2 - n.y) * gravity * this.alpha;
      n.vx *= 0.85;
      n.vy *= 0.85;
      n.x += n.vx;
      n.y += n.vy;
    }

    this.alpha *= 0.985;
  }

  /** Runs the simulation to rest (used for tests and non-animated layouts). */
  run(maxSteps = 400): void {
    for (let i = 0; i < maxSteps && !this.settled; i++) this.step();
  }
}

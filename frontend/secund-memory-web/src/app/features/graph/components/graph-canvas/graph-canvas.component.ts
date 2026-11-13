import {
  AfterViewInit,
  Component,
  ElementRef,
  NgZone,
  OnDestroy,
  effect,
  inject,
  input,
  viewChild,
} from '@angular/core';
import { Router } from '@angular/router';
import { GraphData } from '../../data-access/graph.api';
import { ForceSimulation, SimNode } from '../../data-access/force-simulation';

/**
 * Force-directed graph of pages and their links, drawn on a <canvas>.
 * Hover highlights a node's neighbourhood, drag moves a node, click opens the page.
 */
@Component({
  selector: 'app-graph-canvas',
  standalone: true,
  templateUrl: './graph-canvas.component.html',
  styleUrl: './graph-canvas.component.scss',
})
export class GraphCanvasComponent implements AfterViewInit, OnDestroy {
  readonly data = input.required<GraphData>();

  private readonly canvasRef = viewChild.required<ElementRef<HTMLCanvasElement>>('canvas');
  private readonly router = inject(Router);
  private readonly zone = inject(NgZone);

  private sim!: ForceSimulation;
  private frame = 0;
  private hovered: SimNode | null = null;
  private dragging: SimNode | null = null;
  private moved = false;
  private resizeObserver?: ResizeObserver;
  private width = 800;
  private height = 560;

  private viewReady = false;

  constructor() {
    // Rebuild the simulation when filters change the data, keeping the
    // positions of nodes that stay so the layout doesn't jump.
    effect(() => {
      const data = this.data();
      if (this.viewReady) this.rebuild(data);
    });
  }

  private rebuild(data: GraphData): void {
    const previous = new Map(this.sim?.nodes.map((n) => [n.id, n]));
    this.sim = new ForceSimulation(data, this.width, this.height);
    for (const n of this.sim.nodes) {
      const old = previous.get(n.id);
      if (old) {
        n.x = old.x;
        n.y = old.y;
      }
    }
    this.sim.alpha = previous.size ? 0.6 : 1;
    this.hovered = null;
    this.dragging = null;
    this.kick();
  }

  ngAfterViewInit(): void {
    const canvas = this.canvasRef().nativeElement;
    this.measure(canvas);
    this.sim = new ForceSimulation(this.data(), this.width, this.height);
    this.viewReady = true;

    this.resizeObserver = new ResizeObserver(() => {
      this.measure(canvas);
      this.sim.resize(this.width, this.height);
      this.kick();
    });
    this.resizeObserver.observe(canvas.parentElement ?? canvas);
    this.kick();
  }

  ngOnDestroy(): void {
    cancelAnimationFrame(this.frame);
    this.resizeObserver?.disconnect();
  }

  onMove(ev: PointerEvent): void {
    const p = this.pointer(ev);
    if (this.dragging) {
      this.moved = true;
      this.dragging.x = p.x;
      this.dragging.y = p.y;
      this.dragging.vx = this.dragging.vy = 0;
      this.sim.reheat();
      this.kick();
      return;
    }
    const hit = this.hit(p.x, p.y);
    if (hit !== this.hovered) {
      this.hovered = hit;
      this.canvasRef().nativeElement.style.cursor = hit ? 'pointer' : 'default';
      this.kick();
    }
  }

  onDown(ev: PointerEvent): void {
    const p = this.pointer(ev);
    this.dragging = this.hit(p.x, p.y);
    this.moved = false;
    if (this.dragging) this.canvasRef().nativeElement.setPointerCapture(ev.pointerId);
  }

  onUp(): void {
    const node = this.dragging;
    this.dragging = null;
    if (node && !this.moved) {
      this.router.navigate(['/pages', node.id]);
    }
  }

  onLeave(): void {
    this.hovered = null;
    this.kick();
  }

  private kick(): void {
    if (this.frame) return;
    this.zone.runOutsideAngular(() => {
      const tick = () => {
        this.frame = 0;
        this.sim.step();
        this.draw();
        if (!this.sim.settled || this.dragging) this.frame = requestAnimationFrame(tick);
      };
      this.frame = requestAnimationFrame(tick);
    });
  }

  private measure(canvas: HTMLCanvasElement): void {
    const box = (canvas.parentElement ?? canvas).getBoundingClientRect();
    this.width = Math.max(box.width, 320);
    this.height = Math.max(Math.min(box.width * 0.7, 640), 360);
    const dpr = window.devicePixelRatio || 1;
    canvas.width = this.width * dpr;
    canvas.height = this.height * dpr;
    canvas.style.height = `${this.height}px`;
    canvas.getContext('2d')?.setTransform(dpr, 0, 0, dpr, 0, 0);
  }

  private pointer(ev: PointerEvent): { x: number; y: number } {
    const r = this.canvasRef().nativeElement.getBoundingClientRect();
    return { x: ev.clientX - r.left, y: ev.clientY - r.top };
  }

  private radius(n: SimNode): number {
    return 5 + Math.min(n.degree, 8) * 1.5;
  }

  private hit(x: number, y: number): SimNode | null {
    for (let i = this.sim.nodes.length - 1; i >= 0; i--) {
      const n = this.sim.nodes[i];
      const r = this.radius(n) + 3;
      if ((n.x - x) ** 2 + (n.y - y) ** 2 <= r * r) return n;
    }
    return null;
  }

  private draw(): void {
    const canvas = this.canvasRef().nativeElement;
    const ctx = canvas.getContext('2d');
    if (!ctx) return;
    const css = getComputedStyle(canvas);
    const text = css.getPropertyValue('--color-text').trim() || '#222';
    const muted = css.getPropertyValue('--color-text-muted').trim() || '#888';
    const accent = css.getPropertyValue('--color-accent').trim() || '#4338f0';

    ctx.clearRect(0, 0, this.width, this.height);

    const neighbours = new Set<SimNode>();
    if (this.hovered) {
      neighbours.add(this.hovered);
      for (const e of this.sim.edges) {
        if (e.source === this.hovered) neighbours.add(e.target);
        if (e.target === this.hovered) neighbours.add(e.source);
      }
    }
    const dim = (n: SimNode) => this.hovered !== null && !neighbours.has(n);

    ctx.lineWidth = 1;
    for (const e of this.sim.edges) {
      const active = this.hovered && (e.source === this.hovered || e.target === this.hovered);
      ctx.globalAlpha = this.hovered ? (active ? 0.9 : 0.08) : 0.35;
      ctx.strokeStyle = active ? accent : muted;
      ctx.beginPath();
      ctx.moveTo(e.source.x, e.source.y);
      ctx.lineTo(e.target.x, e.target.y);
      ctx.stroke();
    }

    ctx.font = '12px system-ui, sans-serif';
    ctx.textAlign = 'center';
    for (const n of this.sim.nodes) {
      ctx.globalAlpha = dim(n) ? 0.2 : 1;
      ctx.fillStyle = n === this.hovered ? accent : n.degree > 0 ? text : muted;
      ctx.beginPath();
      ctx.arc(n.x, n.y, this.radius(n), 0, Math.PI * 2);
      ctx.fill();
      if (!dim(n)) {
        ctx.fillStyle = text;
        ctx.fillText(n.title, n.x, n.y + this.radius(n) + 14);
      }
    }
    ctx.globalAlpha = 1;
  }
}

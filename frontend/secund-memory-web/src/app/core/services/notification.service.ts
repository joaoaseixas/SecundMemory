import { Injectable, signal } from '@angular/core';

export type ToastSeverity = 'info' | 'success' | 'error';

export interface Toast {
  id: number;
  message: string;
  severity: ToastSeverity;
}

let nextId = 1;

/** Feeds shared/ui/toast — call show(), the component renders whatever's in toasts(). */
@Injectable({ providedIn: 'root' })
export class NotificationService {
  readonly toasts = signal<Toast[]>([]);

  show(message: string, severity: ToastSeverity = 'info', durationMs = 4000): void {
    const toast: Toast = { id: nextId++, message, severity };
    this.toasts.update((current) => [...current, toast]);

    setTimeout(() => this.dismiss(toast.id), durationMs);
  }

  dismiss(id: number): void {
    this.toasts.update((current) => current.filter((t) => t.id !== id));
  }
}

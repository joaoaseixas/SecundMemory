import { Component, inject, signal } from '@angular/core';
import { StatTileComponent } from '../../shared/components/stat-tile/stat-tile.component';
import { SecondBrainApiService } from '../../core/services/second-brain-api.service';
import { CapacitySummary } from '../../core/models/capacity-summary.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [StatTileComponent],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent {
  private readonly api = inject(SecondBrainApiService);

  protected readonly summary = signal<CapacitySummary | null>(null);
  protected readonly error = signal<string | null>(null);

  constructor() {
    this.api.getCapacitySummary().subscribe({
      next: (summary) => this.summary.set(summary),
      error: () => this.error.set('Could not reach SecundMemory.Api. Is the backend running?'),
    });
  }
}

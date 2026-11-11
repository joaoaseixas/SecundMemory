import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { EmptyStateComponent } from '../../../../shared/ui/empty-state/empty-state.component';
import { Backlink } from '../../models/page.model';

@Component({
  selector: 'app-backlinks-panel',
  standalone: true,
  imports: [RouterLink, EmptyStateComponent],
  templateUrl: './backlinks-panel.component.html',
  styleUrl: './backlinks-panel.component.scss',
})
export class BacklinksPanelComponent {
  readonly backlinks = input.required<Backlink[]>();
}

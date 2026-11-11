import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';

export interface RecentPage {
  id: string;
  title: string;
}

@Component({
  selector: 'app-recent-pages',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './recent-pages.component.html',
  styleUrl: './recent-pages.component.scss',
})
export class RecentPagesComponent {
  readonly pages = input<RecentPage[]>([]);
}

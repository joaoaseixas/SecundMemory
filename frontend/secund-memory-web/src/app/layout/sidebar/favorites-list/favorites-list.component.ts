import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';

export interface FavoritePage {
  id: string;
  title: string;
}

/** Presentational only — favorites data comes from wherever the sidebar is used once features/pages exists. */
@Component({
  selector: 'app-favorites-list',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './favorites-list.component.html',
  styleUrl: './favorites-list.component.scss',
})
export class FavoritesListComponent {
  readonly favorites = input<FavoritePage[]>([]);
}

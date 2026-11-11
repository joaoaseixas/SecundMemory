import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { FavoritesListComponent } from './favorites-list/favorites-list.component';
import { RecentPagesComponent } from './recent-pages/recent-pages.component';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, FavoritesListComponent, RecentPagesComponent],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss',
})
export class SidebarComponent {}

import { Component, OnInit, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { ButtonComponent } from '../../../../shared/ui/button/button.component';
import { InputComponent } from '../../../../shared/ui/input/input.component';
import { EmptyStateComponent } from '../../../../shared/ui/empty-state/empty-state.component';
import { PagesStore } from '../../data-access/pages.store';

@Component({
  selector: 'app-page-list',
  standalone: true,
  imports: [RouterLink, ButtonComponent, InputComponent, EmptyStateComponent],
  templateUrl: './page-list.component.html',
  styleUrl: './page-list.component.scss',
})
export class PageListComponent implements OnInit {
  protected readonly store = inject(PagesStore);
  private readonly router = inject(Router);

  protected readonly newTitle = signal('');

  ngOnInit(): void {
    this.store.loadList();
  }

  createPage(): void {
    const title = this.newTitle().trim();
    if (!title) return;

    this.store.create(title).subscribe((page) => {
      this.newTitle.set('');
      this.router.navigate(['/pages', page.id]);
    });
  }
}

import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PageTitleComponent } from '../../components/page-title/page-title.component';
import { PagePropertiesComponent } from '../../components/page-properties/page-properties.component';
import { BacklinksPanelComponent } from '../../components/backlinks-panel/backlinks-panel.component';
import { BlockTreeComponent } from '../../../editor/components/block-tree/block-tree.component';
import { RightSidebarComponent } from '../../../../layout/right-sidebar/right-sidebar.component';
import { PagesStore } from '../../data-access/pages.store';

@Component({
  selector: 'app-page-view',
  standalone: true,
  imports: [PageTitleComponent, PagePropertiesComponent, BacklinksPanelComponent, BlockTreeComponent, RightSidebarComponent],
  templateUrl: './page-view.component.html',
  styleUrl: './page-view.component.scss',
})
export class PageViewComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  protected readonly store = inject(PagesStore);

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      const id = params.get('id');
      if (id) this.store.loadPage(id);
    });
  }

  rename(newTitle: string): void {
    const page = this.store.currentPage();
    if (page) this.store.rename(page.id, newTitle).subscribe();
  }
}

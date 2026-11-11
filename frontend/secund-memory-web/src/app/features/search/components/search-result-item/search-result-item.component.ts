import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { HighlightPipe } from '../../../../shared/pipes/highlight.pipe';
import { SearchResult } from '../../data-access/search.api';

@Component({
  selector: 'app-search-result-item',
  standalone: true,
  imports: [RouterLink, HighlightPipe],
  templateUrl: './search-result-item.component.html',
  styleUrl: './search-result-item.component.scss',
})
export class SearchResultItemComponent {
  readonly result = input.required<SearchResult>();
  readonly query = input('');
}

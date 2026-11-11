import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Tag } from '../../data-access/tags.api';

@Component({
  selector: 'app-tag-cloud',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './tag-cloud.component.html',
  styleUrl: './tag-cloud.component.scss',
})
export class TagCloudComponent {
  readonly tags = input.required<Tag[]>();
}

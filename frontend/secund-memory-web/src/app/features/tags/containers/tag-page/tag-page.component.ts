import { Component, OnInit, inject, signal } from '@angular/core';
import { ButtonComponent } from '../../../../shared/ui/button/button.component';
import { InputComponent } from '../../../../shared/ui/input/input.component';
import { TagCloudComponent } from '../../components/tag-cloud/tag-cloud.component';
import { Tag, TagsApi } from '../../data-access/tags.api';

@Component({
  selector: 'app-tag-page',
  standalone: true,
  imports: [ButtonComponent, InputComponent, TagCloudComponent],
  templateUrl: './tag-page.component.html',
  styleUrl: './tag-page.component.scss',
})
export class TagPageComponent implements OnInit {
  private readonly tagsApi = inject(TagsApi);
  protected readonly tags = signal<Tag[]>([]);
  protected readonly newTagName = signal('');

  ngOnInit(): void {
    this.tagsApi.list().subscribe((tags) => this.tags.set(tags));
  }

  createTag(): void {
    const name = this.newTagName().trim();
    if (!name) return;

    this.tagsApi.create(name).subscribe((tag) => {
      this.tags.update((tags) => [...tags, tag]);
      this.newTagName.set('');
    });
  }
}

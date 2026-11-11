import { Component, OnInit, inject, signal } from '@angular/core';
import { EmptyStateComponent } from '../../shared/ui/empty-state/empty-state.component';
import { Attachment, AttachmentsApi } from './attachments.api';

/**
 * Lists what's already uploaded (upload happens inline from a block, via
 * AttachmentsController — see /api/attachments). A standalone drag-and-drop
 * uploader here is Fase 2 polish, not required for the list itself to work.
 */
@Component({
  selector: 'app-attachments-list',
  standalone: true,
  imports: [EmptyStateComponent],
  templateUrl: './attachments-list.component.html',
  styleUrl: './attachments-list.component.scss',
})
export class AttachmentsListComponent implements OnInit {
  private readonly attachmentsApi = inject(AttachmentsApi);
  protected readonly attachments = signal<Attachment[]>([]);

  ngOnInit(): void {
    this.attachmentsApi.list().subscribe((attachments) => this.attachments.set(attachments));
  }
}

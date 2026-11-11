import { Pipe, PipeTransform } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

/** Used by search results to bold the matched query inside a snippet. */
@Pipe({ name: 'highlight', standalone: true })
export class HighlightPipe implements PipeTransform {
  constructor(private readonly sanitizer: DomSanitizer) {}

  transform(text: string, query: string): SafeHtml {
    if (!query.trim()) return this.sanitizer.bypassSecurityTrustHtml(text);

    const escaped = query.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
    const highlighted = text.replace(new RegExp(`(${escaped})`, 'ig'), '<mark>$1</mark>');
    return this.sanitizer.bypassSecurityTrustHtml(highlighted);
  }
}

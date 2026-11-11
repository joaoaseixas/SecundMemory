import { Injectable } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

/**
 * Inline rendering only — [[links]], ((embeds)), **bold**, *italic* — for
 * showing a block's content when it's NOT focused. Full Markdown
 * import/export (Fase 1 item #7) is a separate, deliberately deferred
 * concern (docs/adr/0004 on the backend); this never touches that.
 */
@Injectable({ providedIn: 'root' })
export class MarkdownParserService {
  constructor(private readonly sanitizer: DomSanitizer) {}

  renderInline(content: string): SafeHtml {
    const escaped = content
      .replace(/&/g, '&amp;')
      .replace(/</g, '&lt;')
      .replace(/>/g, '&gt;');

    const withLinks = escaped.replace(/\[\[([^[\]]+)]]/g, '<a class="inline-link" data-page-title="$1">$1</a>');
    const withEmbeds = withLinks.replace(/\(\(([0-9a-fA-F-]{36})\)\)/g, '<span class="inline-embed" data-block-id="$1">embed</span>');
    const withBold = withEmbeds.replace(/\*\*([^*]+)\*\*/g, '<strong>$1</strong>');
    const withItalic = withBold.replace(/\*([^*]+)\*/g, '<em>$1</em>');

    return this.sanitizer.bypassSecurityTrustHtml(withItalic);
  }
}

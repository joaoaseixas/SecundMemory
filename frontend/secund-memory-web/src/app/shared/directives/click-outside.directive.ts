import { Directive, ElementRef, inject, output } from '@angular/core';

@Directive({ selector: '[appClickOutside]', standalone: true })
export class ClickOutsideDirective {
  private readonly elementRef = inject(ElementRef<HTMLElement>);
  readonly clickOutside = output<void>();

  constructor() {
    document.addEventListener('click', (event) => {
      if (!this.elementRef.nativeElement.contains(event.target as Node)) {
        this.clickOutside.emit();
      }
    });
  }
}

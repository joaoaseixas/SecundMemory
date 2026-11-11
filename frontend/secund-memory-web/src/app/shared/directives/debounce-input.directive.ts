import { Directive, OnDestroy, OnInit, inject, output } from '@angular/core';
import { ElementRef } from '@angular/core';
import { Subject, debounceTime } from 'rxjs';

/** Used by the editor's content textarea and the search box — both need "stopped typing," not "every keystroke." */
@Directive({ selector: '[appDebounceInput]', standalone: true })
export class DebounceInputDirective implements OnInit, OnDestroy {
  private readonly elementRef = inject(ElementRef<HTMLInputElement | HTMLTextAreaElement>);
  private readonly input$ = new Subject<string>();

  readonly debounceMs = 300;
  readonly debouncedValue = output<string>();

  ngOnInit(): void {
    this.elementRef.nativeElement.addEventListener('input', this.onInput);
    this.input$.pipe(debounceTime(this.debounceMs)).subscribe((value) => this.debouncedValue.emit(value));
  }

  ngOnDestroy(): void {
    this.elementRef.nativeElement.removeEventListener('input', this.onInput);
    this.input$.complete();
  }

  private readonly onInput = (event: Event): void => {
    this.input$.next((event.target as HTMLInputElement).value);
  };
}

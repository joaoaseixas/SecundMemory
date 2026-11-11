import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';
import { CommandPaletteComponent } from './command-palette.component';
import { CommandPaletteState } from './command-palette.state';

describe('CommandPaletteComponent', () => {
  let httpMock: HttpTestingController;
  let state: CommandPaletteState;

  beforeEach(async () => {
    // Fake timers by default so any debounced search left pending at test end
    // never fires for real after teardown — advanced explicitly where a test needs it.
    vi.useFakeTimers();

    await TestBed.configureTestingModule({
      imports: [CommandPaletteComponent],
      providers: [provideHttpClient(), provideHttpClientTesting(), provideRouter([])],
    }).compileComponents();

    httpMock = TestBed.inject(HttpTestingController);
    state = TestBed.inject(CommandPaletteState);
  });

  afterEach(() => {
    httpMock.verify();
    vi.useRealTimers();
  });

  it('lists every nav command when the query is empty', () => {
    state.open();
    const fixture = TestBed.createComponent(CommandPaletteComponent);
    fixture.detectChanges();

    const items = fixture.nativeElement.querySelectorAll('.palette__item');
    expect(items.length).toBe(6);
  });

  it('filters nav commands by label as the query narrows', () => {
    state.open();
    const fixture = TestBed.createComponent(CommandPaletteComponent);
    fixture.detectChanges();

    const input: HTMLInputElement = fixture.nativeElement.querySelector('.palette__input');
    input.value = 'graph';
    input.dispatchEvent(new Event('input'));
    fixture.detectChanges();

    const items: NodeListOf<HTMLElement> = fixture.nativeElement.querySelectorAll('.palette__item');
    expect(items.length).toBe(1);
    expect(items[0].textContent).toContain('Go to Graph');
  });

  it('does not search until the query reaches the minimum length', () => {
    state.open();
    const fixture = TestBed.createComponent(CommandPaletteComponent);
    fixture.detectChanges();

    const input: HTMLInputElement = fixture.nativeElement.querySelector('.palette__input');
    input.value = 'a';
    input.dispatchEvent(new Event('input'));
    fixture.detectChanges();

    httpMock.expectNone((req) => req.url === '/api/search');
  });

  it('merges live search results into the palette after the debounce window', async () => {
    state.open();
    const fixture = TestBed.createComponent(CommandPaletteComponent);
    fixture.detectChanges();

    const input: HTMLInputElement = fixture.nativeElement.querySelector('.palette__input');
    input.value = 'onboarding';
    input.dispatchEvent(new Event('input'));
    fixture.detectChanges();

    await vi.advanceTimersByTimeAsync(300);

    httpMock.expectOne((req) => req.url === '/api/search').flush([
      { id: 'b1', kind: 'Block', pageId: 'p1', title: 'Onboarding', snippet: 'Onboarding steps for new hires', rank: 1 },
    ]);
    fixture.detectChanges();

    const resultItems = fixture.nativeElement.querySelectorAll('.palette__item--result');
    expect(resultItems.length).toBe(1);
    expect(resultItems[0].textContent).toContain('Onboarding steps');
  });
});

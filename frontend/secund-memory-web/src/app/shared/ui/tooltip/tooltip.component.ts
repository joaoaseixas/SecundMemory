import { Component, input } from '@angular/core';

/** CSS-only tooltip — no JS positioning needed for the simple "label on hover" case this covers. */
@Component({
  selector: 'app-tooltip',
  standalone: true,
  templateUrl: './tooltip.component.html',
  styleUrl: './tooltip.component.scss',
})
export class TooltipComponent {
  readonly text = input.required<string>();
}

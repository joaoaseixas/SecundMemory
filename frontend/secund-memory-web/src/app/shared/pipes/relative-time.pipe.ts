import { Pipe, PipeTransform } from '@angular/core';

type Unit = 'second' | 'minute' | 'hour' | 'day' | 'month' | 'year';

const THRESHOLDS: [limitSeconds: number, unit: Unit, divisor: number][] = [
  [60, 'second', 1],
  [3600, 'minute', 60],
  [86400, 'hour', 3600],
  [2_592_000, 'day', 86400],
  [31_536_000, 'month', 2_592_000],
  [Infinity, 'year', 31_536_000],
];

@Pipe({ name: 'relativeTime', standalone: true })
export class RelativeTimePipe implements PipeTransform {
  private readonly formatter = new Intl.RelativeTimeFormat(undefined, { numeric: 'auto' });

  transform(value: string | Date): string {
    const date = typeof value === 'string' ? new Date(value) : value;
    const seconds = Math.round((date.getTime() - Date.now()) / 1000);

    const [, unit, divisor] = THRESHOLDS.find(([limit]) => Math.abs(seconds) < limit) ?? THRESHOLDS[THRESHOLDS.length - 1];
    return this.formatter.format(Math.round(seconds / divisor), unit);
  }
}

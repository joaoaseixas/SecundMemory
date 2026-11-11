import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'truncate', standalone: true })
export class TruncatePipe implements PipeTransform {
  transform(value: string, maxLength = 160): string {
    return value.length > maxLength ? `${value.slice(0, maxLength)}…` : value;
  }
}

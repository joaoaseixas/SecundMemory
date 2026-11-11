import { ErrorHandler, Injectable, inject } from '@angular/core';
import { LoggerService } from '../services/logger.service';
import { NotificationService } from '../services/notification.service';

/** Catches anything that escapes a component's own try/catch — a visible toast beats a silent console error for an end user. */
@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
  private readonly logger = inject(LoggerService);
  private readonly notifications = inject(NotificationService);

  handleError(error: unknown): void {
    this.logger.error('Unhandled error', error);
    this.notifications.show('Something went wrong.', 'error');
  }
}

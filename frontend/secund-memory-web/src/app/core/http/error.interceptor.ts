import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { ApiError } from '../../shared/models/api-error.model';
import { NotificationService } from '../services/notification.service';

/** 401 is handled by refresh-token.interceptor instead — surfacing a toast for it here too would be redundant/confusing mid-refresh. */
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const notifications = inject(NotificationService);

  return next(req).pipe(
    catchError((error: unknown) => {
      if (error instanceof HttpErrorResponse && error.status !== 401) {
        const body = error.error as ApiError | undefined;
        notifications.show(body?.title ?? 'Request failed.', 'error');
      }

      return throwError(() => error);
    }),
  );
};

import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../auth.service';
import { AuthStore } from '../auth.store';

const REFRESH_ENDPOINT_FRAGMENT = '/auth/refresh';

/**
 * One retry only: if the refreshed token also gets a 401, the session is
 * genuinely invalid — looping would just hammer the backend.
 */
export const refreshTokenInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const authStore = inject(AuthStore);
  const router = inject(Router);

  return next(req).pipe(
    catchError((error: unknown) => {
      const isUnauthorized = error instanceof HttpErrorResponse && error.status === 401;
      const isRefreshCall = req.url.includes(REFRESH_ENDPOINT_FRAGMENT);

      if (!isUnauthorized || isRefreshCall || !authStore.isAuthenticated()) {
        return throwError(() => error);
      }

      return authService.refresh().pipe(
        switchMap((result) => next(req.clone({ setHeaders: { Authorization: `Bearer ${result.accessToken}` } }))),
        catchError((refreshError: unknown) => {
          authService.logout();
          router.navigate(['/login']);
          return throwError(() => refreshError);
        }),
      );
    }),
  );
};

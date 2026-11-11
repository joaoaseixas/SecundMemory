import { HttpInterceptorFn } from '@angular/common/http';
import { finalize } from 'rxjs';
import { loadingRequestCount } from './loading.state';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  loadingRequestCount.update((count) => count + 1);

  return next(req).pipe(finalize(() => loadingRequestCount.update((count) => count - 1)));
};

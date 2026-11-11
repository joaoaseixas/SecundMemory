import { computed, signal } from '@angular/core';

/**
 * Module-level signal rather than a service: HttpInterceptorFn runs outside
 * Angular's injector tree for some requests (notably during app
 * initialization), so a plain signal is more reliable here than DI.
 */
export const loadingRequestCount = signal(0);
export const isLoading = computed(() => loadingRequestCount() > 0);

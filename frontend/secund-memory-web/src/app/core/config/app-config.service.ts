import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';

/** Wraps the environment object so the rest of the app depends on this (injectable, mockable) instead of importing environment.ts directly everywhere. */
@Injectable({ providedIn: 'root' })
export class AppConfigService {
  readonly apiBaseUrl = environment.apiBaseUrl;
  readonly production = environment.production;
}

import { Injectable, inject } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { ApiClientService } from '../http/api-client.service';
import { ApiEndpoints } from '../http/api-endpoints';
import { AuthStore } from './auth.store';

export interface AuthResult {
  userId: string;
  workspaceId: string;
  accessToken: string;
  expiresAtUtc: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly api = inject(ApiClientService);
  private readonly store = inject(AuthStore);

  register(email: string, displayName: string, password: string): Observable<AuthResult> {
    return this.api
      .post<AuthResult>(ApiEndpoints.auth.register, { email, displayName, password })
      .pipe(tap((result) => this.persist(result)));
  }

  login(email: string, password: string): Observable<AuthResult> {
    return this.api
      .post<AuthResult>(ApiEndpoints.auth.login, { email, password })
      .pipe(tap((result) => this.persist(result)));
  }

  refresh(): Observable<AuthResult> {
    const expiredAccessToken = this.store.session()?.accessToken ?? '';
    return this.api
      .post<AuthResult>(ApiEndpoints.auth.refresh, { expiredAccessToken })
      .pipe(tap((result) => this.persist(result)));
  }

  logout(): void {
    this.store.clearSession();
  }

  private persist(result: AuthResult): void {
    this.store.setSession({
      accessToken: result.accessToken,
      expiresAtUtc: result.expiresAtUtc,
      userId: result.userId,
      workspaceId: result.workspaceId,
    });
  }
}

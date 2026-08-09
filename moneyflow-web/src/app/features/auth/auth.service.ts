import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { finalize, Observable, of, tap, throwError } from 'rxjs';

import { environment } from '../../../environments/environment';
import { AuthStorageService } from '../../core/auth/auth-storage.service';
import {
  AuthResponse,
  LoginRequest,
  LogoutRequest,
  RefreshTokenRequest,
  RegisterRequest,
} from './auth.models';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly authStorage = inject(AuthStorageService);

  private readonly authUrl = `${environment.apiUrl}/api/v1/auth`;

  login(request: LoginRequest, rememberMe: boolean): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.authUrl}/login`, request)
      .pipe(tap((response) => this.authStorage.saveSession(response, rememberMe)));
  }

  register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.authUrl}/register`, request)
      .pipe(tap((response) => this.authStorage.saveSession(response)));
  }

  refreshSession(): Observable<AuthResponse> {
    const refreshToken = this.authStorage.getRefreshToken();

    if (!refreshToken || this.authStorage.isRefreshTokenExpired()) {
      return throwError(() => new Error('No hay una sesión disponible para renovar.'));
    }

    const request: RefreshTokenRequest = { refreshToken };
    const rememberMe = this.authStorage.isPersistentSession();

    return this.http
      .post<AuthResponse>(`${this.authUrl}/refresh`, request)
      .pipe(tap((response) => this.authStorage.saveSession(response, rememberMe)));
  }

  logout(): Observable<void> {
    const refreshToken = this.authStorage.getRefreshToken();

    if (!refreshToken) {
      this.authStorage.clearSession();
      return of(undefined);
    }

    const request: LogoutRequest = { refreshToken };

    return this.http
      .post<void>(`${this.authUrl}/logout`, request)
      .pipe(finalize(() => this.authStorage.clearSession()));
  }
}

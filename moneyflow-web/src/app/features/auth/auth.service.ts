import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';

import { environment } from '../../../environments/environment';
import { AuthStorageService } from '../../core/auth/auth-storage.service';
import { AuthResponse, LoginRequest, RegisterRequest } from './auth.models';

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

  logout(): void {
    this.authStorage.clearSession();
  }
}

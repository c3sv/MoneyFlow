import { Injectable } from '@angular/core';

import { AuthResponse } from '../../features/auth/auth.models';

@Injectable({
  providedIn: 'root',
})
export class AuthStorageService {
  private readonly storageKey = 'moneyflow_session';

  saveSession(session: AuthResponse, rememberMe = true): void {
    this.clearSession();

    const storage = rememberMe ? localStorage : sessionStorage;

    storage.setItem(this.storageKey, JSON.stringify(session));
  }

  getSession(): AuthResponse | null {
    return this.readSession(localStorage) ?? this.readSession(sessionStorage);
  }

  getToken(): string | null {
    return this.getAccessToken();
  }

  getAccessToken(): string | null {
    return this.getSession()?.accessToken ?? null;
  }

  getRefreshToken(): string | null {
    return this.getSession()?.refreshToken ?? null;
  }

  isAccessTokenExpired(offsetSeconds = 0): boolean {
    const expiresAt = this.getSession()?.accessTokenExpiresAt ?? null;

    return this.isExpired(expiresAt, offsetSeconds);
  }

  isRefreshTokenExpired(): boolean {
    const expiresAt = this.getSession()?.refreshTokenExpiresAt ?? null;

    return this.isExpired(expiresAt);
  }

  isPersistentSession(): boolean {
    return localStorage.getItem(this.storageKey) !== null;
  }

  isAuthenticated(): boolean {
    return Boolean(this.getRefreshToken()) && !this.isRefreshTokenExpired();
  }

  clearSession(): void {
    localStorage.removeItem(this.storageKey);
    sessionStorage.removeItem(this.storageKey);
  }

  private readSession(storage: Storage): AuthResponse | null {
    const storedSession = storage.getItem(this.storageKey);

    if (!storedSession) {
      return null;
    }

    try {
      return JSON.parse(storedSession) as AuthResponse;
    } catch {
      storage.removeItem(this.storageKey);
      return null;
    }
  }

  private isExpired(expiresAt: string | null, offsetSeconds = 0): boolean {
    if (!expiresAt) {
      return true;
    }

    const expirationTime = Date.parse(expiresAt);

    if (Number.isNaN(expirationTime)) {
      return true;
    }

    return expirationTime <= Date.now() + offsetSeconds * 1000;
  }
}

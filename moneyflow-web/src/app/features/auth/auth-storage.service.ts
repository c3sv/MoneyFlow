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
    return this.getSession()?.token ?? null;
  }

  isAuthenticated(): boolean {
    return Boolean(this.getToken());
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
}

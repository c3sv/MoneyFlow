import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';

import { environment } from '../../../environments/environment';
import { AuthStorageService } from '../../core/auth/auth-storage.service';
import { AuthResponse } from './auth.models';
import { AuthService } from './auth.service';

describe('AuthService', () => {
  const authUrl = `${environment.apiUrl}/api/v1/auth`;

  let service: AuthService;
  let httpTesting: HttpTestingController;
  let authStorage: {
    saveSession: ReturnType<typeof vi.fn>;
    getRefreshToken: ReturnType<typeof vi.fn>;
    isRefreshTokenExpired: ReturnType<typeof vi.fn>;
    isPersistentSession: ReturnType<typeof vi.fn>;
    clearSession: ReturnType<typeof vi.fn>;
  };

  const session: AuthResponse = {
    userId: 1,
    firstName: 'Carlos',
    lastName: 'Mansilla',
    email: 'carlos@example.com',
    accessToken: 'access-token',
    accessTokenExpiresAt: '2026-08-08T17:00:00Z',
    refreshToken: 'refresh-token',
    refreshTokenExpiresAt: '2026-08-15T17:00:00Z',
  };

  beforeEach(() => {
    authStorage = {
      saveSession: vi.fn(),
      getRefreshToken: vi.fn(),
      isRefreshTokenExpired: vi.fn(),
      isPersistentSession: vi.fn(),
      clearSession: vi.fn(),
    };

    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        AuthService,
        { provide: AuthStorageService, useValue: authStorage },
      ],
    });

    service = TestBed.inject(AuthService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTesting.verify();
  });

  it('logs in and stores the session with the selected persistence', () => {
    const request = { email: 'carlos@example.com', password: 'Secret123!' };

    service.login(request, false).subscribe((response) => {
      expect(response).toEqual(session);
    });

    const httpRequest = httpTesting.expectOne(`${authUrl}/login`);
    expect(httpRequest.request.method).toBe('POST');
    expect(httpRequest.request.body).toEqual(request);
    httpRequest.flush(session);

    expect(authStorage.saveSession).toHaveBeenCalledWith(session, false);
  });

  it('refreshes the tokens and preserves the storage type', () => {
    authStorage.getRefreshToken.mockReturnValue('old-refresh-token');
    authStorage.isRefreshTokenExpired.mockReturnValue(false);
    authStorage.isPersistentSession.mockReturnValue(false);

    service.refreshSession().subscribe((response) => {
      expect(response).toEqual(session);
    });

    const httpRequest = httpTesting.expectOne(`${authUrl}/refresh`);
    expect(httpRequest.request.method).toBe('POST');
    expect(httpRequest.request.body).toEqual({ refreshToken: 'old-refresh-token' });
    httpRequest.flush(session);

    expect(authStorage.saveSession).toHaveBeenCalledWith(session, false);
  });

  it('rejects refresh when there is no renewable session', () => {
    authStorage.getRefreshToken.mockReturnValue(null);

    service.refreshSession().subscribe({
      next: () => expect.fail('The refresh should have failed'),
      error: (error: Error) => {
        expect(error.message).toContain('No hay una sesión');
      },
    });
  });

  it('logs out through the API and clears the local session', () => {
    authStorage.getRefreshToken.mockReturnValue('refresh-token');

    service.logout().subscribe();

    const httpRequest = httpTesting.expectOne(`${authUrl}/logout`);
    expect(httpRequest.request.method).toBe('POST');
    expect(httpRequest.request.body).toEqual({ refreshToken: 'refresh-token' });
    httpRequest.flush(null, { status: 204, statusText: 'No Content' });

    expect(authStorage.clearSession).toHaveBeenCalledOnce();
  });

  it('clears the local session without an HTTP call when no refresh token exists', () => {
    authStorage.getRefreshToken.mockReturnValue(null);

    service.logout().subscribe();

    expect(authStorage.clearSession).toHaveBeenCalledOnce();
  });
});

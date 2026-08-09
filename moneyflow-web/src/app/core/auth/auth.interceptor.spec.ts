import { HttpClient, provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { of, Subject, throwError } from 'rxjs';

import { environment } from '../../../environments/environment';
import { AuthResponse } from '../../features/auth/auth.models';
import { AuthService } from '../../features/auth/auth.service';
import { authInterceptor } from './auth.interceptor';
import { AuthStorageService } from './auth-storage.service';

describe('authInterceptor', () => {
  const apiUrl = `${environment.apiUrl}/api/v1`;

  let httpTesting: HttpTestingController;
  let http: HttpClient;
  let authStorage: {
    isAccessTokenExpired: ReturnType<typeof vi.fn>;
    getRefreshToken: ReturnType<typeof vi.fn>;
    isRefreshTokenExpired: ReturnType<typeof vi.fn>;
    getAccessToken: ReturnType<typeof vi.fn>;
    clearSession: ReturnType<typeof vi.fn>;
  };
  let authService: { refreshSession: ReturnType<typeof vi.fn> };
  let router: { url: string; navigate: ReturnType<typeof vi.fn> };

  const renewedSession: AuthResponse = {
    userId: 1,
    firstName: 'Carlos',
    lastName: 'Mansilla',
    email: 'carlos@example.com',
    accessToken: 'new-access-token',
    accessTokenExpiresAt: '2026-08-08T18:00:00Z',
    refreshToken: 'new-refresh-token',
    refreshTokenExpiresAt: '2026-08-15T18:00:00Z',
  };

  beforeEach(() => {
    authStorage = {
      isAccessTokenExpired: vi.fn().mockReturnValue(false),
      getRefreshToken: vi.fn().mockReturnValue('refresh-token'),
      isRefreshTokenExpired: vi.fn().mockReturnValue(false),
      getAccessToken: vi.fn().mockReturnValue('old-access-token'),
      clearSession: vi.fn(),
    };
    authService = { refreshSession: vi.fn() };
    router = { url: '/dashboard/accounts', navigate: vi.fn() };

    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([authInterceptor])),
        provideHttpClientTesting(),
        { provide: AuthStorageService, useValue: authStorage },
        { provide: AuthService, useValue: authService },
        { provide: Router, useValue: router },
      ],
    });

    httpTesting = TestBed.inject(HttpTestingController);
    http = TestBed.inject(HttpClient);
  });

  afterEach(() => {
    httpTesting.verify();
  });

  it('adds the access token to protected API requests', () => {
    http.get(`${apiUrl}/accounts`).subscribe();

    const request = httpTesting.expectOne(`${apiUrl}/accounts`);
    expect(request.request.headers.get('Authorization')).toBe('Bearer old-access-token');
    request.flush([]);
  });

  it('does not add a token to public authentication requests', () => {
    http.post(`${apiUrl}/auth/login`, {}).subscribe();

    const request = httpTesting.expectOne(`${apiUrl}/auth/login`);
    expect(request.request.headers.has('Authorization')).toBe(false);
    request.flush(renewedSession);
  });

  it('refreshes before sending a request when the access token is expiring', () => {
    authStorage.isAccessTokenExpired.mockReturnValue(true);
    authService.refreshSession.mockReturnValue(of(renewedSession));

    http.get(`${apiUrl}/accounts`).subscribe();

    const request = httpTesting.expectOne(`${apiUrl}/accounts`);
    expect(authService.refreshSession).toHaveBeenCalledOnce();
    expect(request.request.headers.get('Authorization')).toBe('Bearer new-access-token');
    request.flush([]);
  });

  it('retries once with a renewed token after a 401 response', () => {
    authService.refreshSession.mockReturnValue(of(renewedSession));

    http.get(`${apiUrl}/accounts`).subscribe();

    const firstRequest = httpTesting.expectOne(`${apiUrl}/accounts`);
    expect(firstRequest.request.headers.get('Authorization')).toBe('Bearer old-access-token');
    firstRequest.flush(null, { status: 401, statusText: 'Unauthorized' });

    const retriedRequest = httpTesting.expectOne(`${apiUrl}/accounts`);
    expect(retriedRequest.request.headers.get('Authorization')).toBe('Bearer new-access-token');
    retriedRequest.flush([]);

    expect(authService.refreshSession).toHaveBeenCalledOnce();
  });

  it('shares one refresh between simultaneous requests', () => {
    const refreshResult = new Subject<AuthResponse>();
    authStorage.isAccessTokenExpired.mockReturnValue(true);
    authService.refreshSession.mockReturnValue(refreshResult.asObservable());

    http.get(`${apiUrl}/accounts`).subscribe();
    http.get(`${apiUrl}/categories`).subscribe();

    expect(authService.refreshSession).toHaveBeenCalledOnce();
    refreshResult.next(renewedSession);
    refreshResult.complete();

    const requests = httpTesting.match(
      (request) => request.url === `${apiUrl}/accounts` || request.url === `${apiUrl}/categories`,
    );
    expect(requests).toHaveLength(2);
    requests.forEach((request) => {
      expect(request.request.headers.get('Authorization')).toBe('Bearer new-access-token');
      request.flush([]);
    });
  });

  it('clears the session and returns to login when refresh fails', () => {
    authStorage.isAccessTokenExpired.mockReturnValue(true);
    authService.refreshSession.mockReturnValue(throwError(() => new Error('Refresh failed')));

    http.get(`${apiUrl}/accounts`).subscribe({ error: () => undefined });

    expect(authStorage.clearSession).toHaveBeenCalledOnce();
    expect(router.navigate).toHaveBeenCalledWith(['/login'], {
      queryParams: {
        sessionExpired: 1,
        returnUrl: '/dashboard/accounts',
      },
    });
  });
});

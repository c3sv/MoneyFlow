import { TestBed } from '@angular/core/testing';
import { ActivatedRouteSnapshot, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { firstValueFrom, Observable, of, throwError } from 'rxjs';

import { AuthService } from '../../features/auth/auth.service';
import { authGuard } from './auth.guard';
import { AuthStorageService } from './auth-storage.service';

describe('authGuard', () => {
  let authStorage: {
    isAuthenticated: ReturnType<typeof vi.fn>;
    isAccessTokenExpired: ReturnType<typeof vi.fn>;
    clearSession: ReturnType<typeof vi.fn>;
  };
  let authService: { refreshSession: ReturnType<typeof vi.fn> };
  let router: { createUrlTree: ReturnType<typeof vi.fn> };
  let loginUrl: UrlTree;

  const runGuard = () =>
    TestBed.runInInjectionContext(() =>
      authGuard(
        {} as ActivatedRouteSnapshot,
        { url: '/dashboard/accounts' } as RouterStateSnapshot,
      ),
    );

  beforeEach(() => {
    loginUrl = {} as UrlTree;
    authStorage = {
      isAuthenticated: vi.fn(),
      isAccessTokenExpired: vi.fn(),
      clearSession: vi.fn(),
    };
    authService = { refreshSession: vi.fn() };
    router = { createUrlTree: vi.fn().mockReturnValue(loginUrl) };

    TestBed.configureTestingModule({
      providers: [
        { provide: AuthStorageService, useValue: authStorage },
        { provide: AuthService, useValue: authService },
        { provide: Router, useValue: router },
      ],
    });
  });

  it('redirects unauthenticated users and preserves the requested URL', () => {
    authStorage.isAuthenticated.mockReturnValue(false);

    expect(runGuard()).toBe(loginUrl);
    expect(authStorage.clearSession).toHaveBeenCalledOnce();
    expect(router.createUrlTree).toHaveBeenCalledWith(['/login'], {
      queryParams: { returnUrl: '/dashboard/accounts' },
    });
  });

  it('allows navigation when the access token is valid', () => {
    authStorage.isAuthenticated.mockReturnValue(true);
    authStorage.isAccessTokenExpired.mockReturnValue(false);

    expect(runGuard()).toBe(true);
    expect(authService.refreshSession).not.toHaveBeenCalled();
  });

  it('renews an expiring token before allowing navigation', async () => {
    authStorage.isAuthenticated.mockReturnValue(true);
    authStorage.isAccessTokenExpired.mockReturnValue(true);
    authService.refreshSession.mockReturnValue(of({}));

    const result = await firstValueFrom(runGuard() as Observable<boolean | UrlTree>);

    expect(result).toBe(true);
    expect(authService.refreshSession).toHaveBeenCalledOnce();
  });

  it('redirects with a session-expired flag when renewal fails', async () => {
    authStorage.isAuthenticated.mockReturnValue(true);
    authStorage.isAccessTokenExpired.mockReturnValue(true);
    authService.refreshSession.mockReturnValue(throwError(() => new Error('Refresh failed')));

    const result = await firstValueFrom(runGuard() as Observable<boolean | UrlTree>);

    expect(result).toBe(loginUrl);
    expect(authStorage.clearSession).toHaveBeenCalledOnce();
    expect(router.createUrlTree).toHaveBeenCalledWith(['/login'], {
      queryParams: {
        returnUrl: '/dashboard/accounts',
        sessionExpired: 1,
      },
    });
  });
});

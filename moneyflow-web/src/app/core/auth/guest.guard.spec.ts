import { TestBed } from '@angular/core/testing';
import { ActivatedRouteSnapshot, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { firstValueFrom, Observable, of, throwError } from 'rxjs';

import { AuthService } from '../../features/auth/auth.service';
import { guestGuard } from './guest.guard';
import { AuthStorageService } from './auth-storage.service';

describe('guestGuard', () => {
  let authStorage: {
    isAuthenticated: ReturnType<typeof vi.fn>;
    isAccessTokenExpired: ReturnType<typeof vi.fn>;
    clearSession: ReturnType<typeof vi.fn>;
  };
  let authService: { refreshSession: ReturnType<typeof vi.fn> };
  let router: { createUrlTree: ReturnType<typeof vi.fn> };
  let dashboardUrl: UrlTree;

  const runGuard = () =>
    TestBed.runInInjectionContext(() =>
      guestGuard({} as ActivatedRouteSnapshot, {} as RouterStateSnapshot),
    );

  beforeEach(() => {
    dashboardUrl = {} as UrlTree;
    authStorage = {
      isAuthenticated: vi.fn(),
      isAccessTokenExpired: vi.fn(),
      clearSession: vi.fn(),
    };
    authService = { refreshSession: vi.fn() };
    router = { createUrlTree: vi.fn().mockReturnValue(dashboardUrl) };

    TestBed.configureTestingModule({
      providers: [
        { provide: AuthStorageService, useValue: authStorage },
        { provide: AuthService, useValue: authService },
        { provide: Router, useValue: router },
      ],
    });
  });

  it('allows access to login when there is no valid session', () => {
    authStorage.isAuthenticated.mockReturnValue(false);

    expect(runGuard()).toBe(true);
    expect(authStorage.clearSession).toHaveBeenCalledOnce();
  });

  it('redirects authenticated users to the dashboard', () => {
    authStorage.isAuthenticated.mockReturnValue(true);
    authStorage.isAccessTokenExpired.mockReturnValue(false);

    expect(runGuard()).toBe(dashboardUrl);
    expect(router.createUrlTree).toHaveBeenCalledWith(['/dashboard']);
  });

  it('renews an expiring session before redirecting to the dashboard', async () => {
    authStorage.isAuthenticated.mockReturnValue(true);
    authStorage.isAccessTokenExpired.mockReturnValue(true);
    authService.refreshSession.mockReturnValue(of({}));

    const result = await firstValueFrom(runGuard() as Observable<boolean | UrlTree>);

    expect(result).toBe(dashboardUrl);
    expect(authService.refreshSession).toHaveBeenCalledOnce();
  });

  it('allows login and clears the session when renewal fails', async () => {
    authStorage.isAuthenticated.mockReturnValue(true);
    authStorage.isAccessTokenExpired.mockReturnValue(true);
    authService.refreshSession.mockReturnValue(throwError(() => new Error('Refresh failed')));

    const result = await firstValueFrom(runGuard() as Observable<boolean | UrlTree>);

    expect(result).toBe(true);
    expect(authStorage.clearSession).toHaveBeenCalledOnce();
  });
});

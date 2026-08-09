import { inject } from '@angular/core';
import { CanActivateFn, Router, UrlTree } from '@angular/router';
import { catchError, map, Observable, of } from 'rxjs';

import { AuthService } from '../../features/auth/auth.service';
import { AuthStorageService } from './auth-storage.service';

export const authGuard: CanActivateFn = (_route, state) => {
  const authService = inject(AuthService);
  const authStorage = inject(AuthStorageService);
  const router = inject(Router);

  const loginUrl = (sessionExpired = false): UrlTree =>
    router.createUrlTree(['/login'], {
      queryParams: {
        returnUrl: state.url,
        ...(sessionExpired && { sessionExpired: 1 }),
      },
    });

  if (!authStorage.isAuthenticated()) {
    authStorage.clearSession();
    return loginUrl();
  }

  if (!authStorage.isAccessTokenExpired(30)) {
    return true;
  }

  return authService.refreshSession().pipe(
    map(() => true),
    catchError((): Observable<UrlTree> => {
      authStorage.clearSession();
      return of(loginUrl(true));
    }),
  );
};

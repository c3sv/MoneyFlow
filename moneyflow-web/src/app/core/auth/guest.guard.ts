import { inject } from '@angular/core';
import { CanActivateFn, Router, UrlTree } from '@angular/router';
import { catchError, map, Observable, of } from 'rxjs';

import { AuthService } from '../../features/auth/auth.service';
import { AuthStorageService } from './auth-storage.service';

export const guestGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const authStorage = inject(AuthStorageService);
  const router = inject(Router);

  const dashboardUrl = (): UrlTree => router.createUrlTree(['/dashboard']);

  if (!authStorage.isAuthenticated()) {
    authStorage.clearSession();
    return true;
  }

  if (!authStorage.isAccessTokenExpired(30)) {
    return dashboardUrl();
  }

  return authService.refreshSession().pipe(
    map(() => dashboardUrl()),
    catchError((): Observable<boolean> => {
      authStorage.clearSession();
      return of(true);
    }),
  );
};

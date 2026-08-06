import { inject } from '@angular/core';
import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

import { environment } from '../../../environments/environment';
import { AuthStorageService } from './auth-storage.service';

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const authStorage = inject(AuthStorageService);
  const router = inject(Router);
  const token = authStorage.getToken();

  const isApiRequest = request.url.startsWith(environment.apiUrl);
  const isAuthenticationRequest =
    request.url.endsWith('/auth/login') || request.url.endsWith('/auth/register');

  const authenticatedRequest =
    token && isApiRequest && !isAuthenticationRequest
      ? request.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
      : request;

  return next(authenticatedRequest).pipe(
    catchError((error: unknown) => {
      if (
        error instanceof HttpErrorResponse &&
        error.status === 401 &&
        isApiRequest &&
        !isAuthenticationRequest
      ) {
        authStorage.clearSession();
        void router.navigateByUrl('/login?sessionExpired=1');
      }

      return throwError(() => error);
    }),
  );
};

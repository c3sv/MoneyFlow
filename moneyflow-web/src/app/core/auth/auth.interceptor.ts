import { HttpErrorResponse, HttpEvent, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, finalize, Observable, shareReplay, switchMap, throwError } from 'rxjs';

import { environment } from '../../../environments/environment';
import { AuthResponse } from '../../features/auth/auth.models';
import { AuthService } from '../../features/auth/auth.service';
import { AuthStorageService } from './auth-storage.service';

let refreshRequest$: Observable<AuthResponse> | null = null;

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  if (!isApiRequest(request) || isPublicAuthenticationRequest(request)) {
    return next(request);
  }

  const authService = inject(AuthService);
  const authStorage = inject(AuthStorageService);
  const router = inject(Router);

  const retryOnce = (accessToken: string): Observable<HttpEvent<unknown>> =>
    next(addAccessToken(request, accessToken)).pipe(
      catchError((error: unknown) => {
        if (error instanceof HttpErrorResponse && error.status === 401) {
          expireSession(authStorage, router);
        }

        return throwError(() => error);
      }),
    );

  const refreshAndRetry = (): Observable<HttpEvent<unknown>> =>
    getRefreshRequest(authService).pipe(
      catchError((error: unknown) => {
        expireSession(authStorage, router);
        return throwError(() => error);
      }),
      switchMap((session) => retryOnce(session.accessToken)),
    );

  if (
    authStorage.isAccessTokenExpired(30) &&
    authStorage.getRefreshToken() &&
    !authStorage.isRefreshTokenExpired()
  ) {
    return refreshAndRetry();
  }

  const accessToken = authStorage.getAccessToken();

  return next(addAccessToken(request, accessToken)).pipe(
    catchError((error: unknown) => {
      if (error instanceof HttpErrorResponse && error.status === 401) {
        const currentAccessToken = authStorage.getAccessToken();

        if (currentAccessToken && currentAccessToken !== accessToken) {
          return retryOnce(currentAccessToken);
        }

        return refreshAndRetry();
      }

      return throwError(() => error);
    }),
  );
};

function getRefreshRequest(authService: AuthService): Observable<AuthResponse> {
  if (!refreshRequest$) {
    refreshRequest$ = authService.refreshSession().pipe(
      finalize(() => {
        refreshRequest$ = null;
      }),
      shareReplay({ bufferSize: 1, refCount: false }),
    );
  }

  return refreshRequest$;
}

function addAccessToken(
  request: HttpRequest<unknown>,
  accessToken: string | null,
): HttpRequest<unknown> {
  if (!accessToken) {
    return request;
  }

  return request.clone({
    setHeaders: {
      Authorization: `Bearer ${accessToken}`,
    },
  });
}

function isApiRequest(request: HttpRequest<unknown>): boolean {
  return request.url.startsWith(`${environment.apiUrl}/api/`);
}

function isPublicAuthenticationRequest(request: HttpRequest<unknown>): boolean {
  return (
    request.url.endsWith('/auth/login') ||
    request.url.endsWith('/auth/register') ||
    request.url.endsWith('/auth/refresh')
  );
}

function expireSession(authStorage: AuthStorageService, router: Router): void {
  const returnUrl = router.url;

  authStorage.clearSession();

  void router.navigate(['/login'], {
    queryParams: {
      sessionExpired: 1,
      ...(returnUrl !== '/login' && { returnUrl }),
    },
  });
}

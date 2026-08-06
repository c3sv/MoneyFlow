import { inject } from '@angular/core';
import { HttpInterceptorFn } from '@angular/common/http';

import { environment } from '../../../environments/environment';
import { AuthStorageService } from './auth-storage.service';

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const authStorage = inject(AuthStorageService);
  const token = authStorage.getToken();

  const isApiRequest = request.url.startsWith(environment.apiUrl);
  const isAuthenticationRequest =
    request.url.endsWith('/auth/login') || request.url.endsWith('/auth/register');

  if (!token || !isApiRequest || isAuthenticationRequest) {
    return next(request);
  }

  const authenticatedRequest = request.clone({
    setHeaders: {
      Authorization: `Bearer ${token}`,
    },
  });

  return next(authenticatedRequest);
};

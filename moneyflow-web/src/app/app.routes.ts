import { Routes } from '@angular/router';

import { authGuard } from './core/auth/auth.guard';
import { guestGuard } from './core/auth/guest.guard';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'dashboard',
  },
  {
    path: 'login',
    title: 'Iniciar sesión | MoneyFlow',
    canActivate: [guestGuard],
    loadComponent: () =>
      import('./features/auth/pages/login/login.component').then(
        (component) => component.LoginComponent,
      ),
  },
  {
    path: 'register',
    title: 'Crear cuenta | MoneyFlow',
    canActivate: [guestGuard],
    loadComponent: () =>
      import('./features/auth/pages/register/register.component').then(
        (component) => component.Register,
      ),
  },
  {
    path: 'dashboard',
    title: 'Dashboard | MoneyFlow',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/dashboard/pages/dashboard/dashboard.component').then(
        (component) => component.Dashboard,
      ),
  },
  {
    path: '**',
    redirectTo: 'dashboard',
  },
];

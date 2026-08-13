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
        (component) => component.RegisterComponent,
      ),
  },
  {
    path: 'dashboard',
    title: 'Dashboard | MoneyFlow',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/dashboard/pages/dashboard/dashboard.component').then(
        (component) => component.DashboardComponent,
      ),
    children: [
      {
        path: 'summary',
        title: 'Summary | MoneyFlow',
        loadComponent: () =>
          import('./features/dashboard/components/summary/summary.component').then(
            (component) => component.SummaryComponent,
          ),
      },
      {
        path: 'accounts',
        title: 'Accounts | MoneyFlow',
        loadComponent: () =>
          import('./features/accounts/pages/accounts/accounts.component').then(
            (component) => component.AccountsComponent,
          ),
      },
      {
        path: 'transactions',
        title: 'Transactions | MoneyFlow',
        loadComponent: () =>
          import('./features/transactions/pages/transactions/transactions.component').then(
            (component) => component.TransactionsComponent,
          ),
      },
      {
        path: 'categories',
        title: 'Categorías | MoneyFlow',
        loadComponent: () =>
          import('./features/categories/pages/categories/categories.component').then(
            (component) => component.CategoriesComponent,
          ),
      },
      { path: '', pathMatch: 'full', redirectTo: 'summary' },
    ],
  },
  {
    path: '**',
    redirectTo: 'dashboard',
  },
];

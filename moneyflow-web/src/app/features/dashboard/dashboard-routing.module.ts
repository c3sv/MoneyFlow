import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { Routes } from '@angular/router';
import { SummaryComponent } from './components/summary/summary.component';

const routes: Routes = [
  {
    path: 'dashboard',
    component: DashboardComponent,
    children: [
      { path: 'summary', component: SummaryComponent }, // sin "/" al inicio
      { path: '', redirectTo: 'summary', pathMatch: 'full' }, // opcional: entrar a /dashboard te lleva a /resumen
    ],
  },
];

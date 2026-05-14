import { Routes } from '@angular/router';
import { ExchangeRateDashboard } from './exchange-rate-dashboard/exchange-rate-dashboard';

export const routes: Routes = [
  { path: '', redirectTo: 'rates', pathMatch: 'full' },
  { path: 'rates', component: ExchangeRateDashboard },
];

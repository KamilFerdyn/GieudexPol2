import { Routes } from '@angular/router';
import { AuthGuard } from './features/auth/guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'auth/login', pathMatch: 'full' },
  { path: 'auth', loadChildren: () => import('./features/auth/auth.module').then(m => m.AuthModule) },
  { path: 'rates', loadChildren: () => import('./exchange-rate-dashboard/exchange-rate-dashboard.module').then(m => m.ExchangeRateDashboardModule), canActivate: [AuthGuard] },
  { path: 'exchange', loadChildren: () => import('./currency-exchange/currency-converter/currency-converter.module').then(m => m.CurrencyConverterModule), canActivate: [AuthGuard] },
  { path: '**', redirectTo: 'auth/login' } // Obsługa nieznanych tras
];

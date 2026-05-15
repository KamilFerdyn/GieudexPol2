import { Routes } from '@angular/router';
import { ExchangeRateDashboard } from './exchange-rate-dashboard/exchange-rate-dashboard';
import { CurrencyConverterComponent } from './currency-exchange/currency-converter/currency-converter';

export const routes: Routes = [
  { path: '', redirectTo: 'rates', pathMatch: 'full' },
  { path: 'rates', component: ExchangeRateDashboard },
  { path: 'exchange', component: CurrencyConverterComponent },
];

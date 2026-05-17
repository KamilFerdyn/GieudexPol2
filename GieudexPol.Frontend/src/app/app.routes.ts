import { Routes } from '@angular/router';
import { AuthGuard } from './features/auth/guards/auth.guard';

import { WalletManagementComponent } from './features/wallet/components/wallet-management/wallet-management.component';
// Import komponentu Dashboard (założenie ścieżki)
import { DashboardComponent } from './components/dashboard/dashboard.component'; 

export const routes: Routes = [
  { path: '', redirectTo: 'auth/login', pathMatch: 'full' },
  // Trasa dla Dashboardu - główny widok po zalogowaniu
  { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard] }, 
  // Dodanie trasy dla zarządzania portfelem
  { path: 'wallet', component: WalletManagementComponent, canActivate: [AuthGuard] }, 
  // ... inne istniejące trasy
];

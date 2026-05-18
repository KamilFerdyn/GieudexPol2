import { Routes } from '@angular/router';
import { AuthGuard } from './features/auth/guards/auth.guard';
 
// Import zaawansowanego komponentu logowania z dedykowanego folderu
import { LoginComponent } from './features/auth/login/login.component';
 
import { WalletManagementComponent } from './features/wallet/components/wallet-management/wallet-management.component';
// Import komponentu Dashboard (założenie ścieżki)
import { DashboardComponent } from './components/dashboard/dashboard.component'; 
 
export const routes: Routes = [
  { path: '', redirectTo: 'auth/login', pathMatch: 'full' },
  // Trasa dla Dashboardu - główny widok po zalogowaniu
  { path: 'auth/login', component: LoginComponent }, 
  // Nowa trasa deweloperska do testów widoku portfela (bez autoryzacji)
  { path: 'test-wallet', component: WalletManagementComponent }, 
  // Trasa dla Dashboardu - główny widok po zalogowaniu (wymaga autoryzacji)
  { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard] }, 
  // Dodanie trasy dla zarządzania portfelem (MUSI zachować AuthGuard!)
  { path: 'wallet', component: WalletManagementComponent, canActivate: [AuthGuard] }, 
  // ... inne istniejące trasy
];
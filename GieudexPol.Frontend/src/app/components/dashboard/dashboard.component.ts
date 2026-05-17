import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common'; // Potrzebne dla *ngIf i *ngFor
import { WalletService } from '../../features/wallet/services/wallet.service';
import { AuthService } from '../../features/auth/services/auth.service';
@Component({
  selector: 'app-dashboard',
  standalone: true, 
  imports: [CommonModule], // CommonModule jest wystarczający dla *ngIf itp.
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent implements OnInit {
  // Saldo i dane użytkownika
  userEmail: string = '';
  currentBalance: { [key: string]: number } = {};
  availableCurrencies: string[] = []; // Nowa tablica do przechowywania kluczy walut
  isLoading: boolean = true;
  constructor(private walletService: WalletService, private authService: AuthService) {} 
  ngOnInit(): void {
    this.loadDashboardData();
  }
  async loadDashboardData(): Promise<void> {
    // Pobieranie danych użytkownika z pamięci lokalnej (z AuthService)
    this.userEmail = localStorage.getItem('userEmail') || 'Niewiadoma'; 
    console.log('Ładowanie danych Dashboardu...');
    await new Promise(resolve => setTimeout(resolve, 500)); // Symulowany delay API
    // Wywołanie usługi do pobrania aktualnego salda portfela
    // Zakładamy, że WalletService ma metodę getBalance() zwracającą { [key: string]: number }
    this.currentBalance = await this.walletService.getBalance(); 
    
    if (this.currentBalance) {
        this.availableCurrencies = Object.keys(this.currentBalance); // Uzupełniamy listę walut
    } else {
        this.availableCurrencies = [];
    }
    this.isLoading = false;
  }
  logout(): void {
    this.authService.logout();
  }
}
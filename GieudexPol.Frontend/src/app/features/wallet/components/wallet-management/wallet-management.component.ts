import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
// Importujemy FormsModule do obsługi dwukierunkowego wiązania danych (ngModel) w szablonie
import { FormsModule } from '@angular/forms'; 
import { WalletService } from '../../services/wallet.service';
import { TradeRequest, TradeResponse } from '../../models/wallet-models';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-wallet-management',
  standalone: true, 
  // Dodajemy FormsModule do importów komponentu
  imports: [CommonModule, FormsModule], 
  templateUrl: './wallet-management.component.html',
  styleUrls: ['./wallet-management.component.scss']
})
export class WalletManagementComponent implements OnInit {
  // Deklaracja właściwości używanych w formularzu (TS2339)
  fromCurrency: { value: string, name: string } = { value: 'PLN', name: 'PLN' }; 
  toCurrency: { value: string, name: string } = { value: 'EUR', name: 'EUR' };
  amount: number | null = null;

  // Stan zarządzany w komponencie (bez zmian)
  availableCurrencies: string[] = ['PLN', 'EUR', 'USD']; 
  currentBalance: { [key: string]: number } = {};
  isLoading: boolean = false;
  tradeMessage: string | null = null;

  constructor(private walletService: WalletService) {}

  async ngOnInit(): Promise<void> {
    await this.initializeBalance(); 
  }

  /**
   * Pobiera i ustawia bieżący stan portfela z API, aktualizując komponent.
   */
  async initializeBalance(): Promise<void> {
    this.isLoading = true;
    try {
      // Użycie await na metodzie getBalance() - jest to czysty async/await
      this.currentBalance = await this.walletService.getBalance(); 
    } catch (error) {
      console.error('Nie udało się załadować salda:', error);
      this.tradeMessage = 'Błąd: Nie można załadować danych konta.';
    } finally {
      this.isLoading = false;
    }
  }

/**
   * Obsługa formularza transakcji wymiany walut.
   */
async executeTrade(fromCurrency: string, toCurrency: string, amount: number | null): Promise<void> {
    // 1. Walidacja na początku - kwota musi być poprawną liczbą > 0
    if (amount === null || amount <= 0) {
      this.tradeMessage = 'Kwota musi być większa od zera.';
      return;
    }

    const numericAmount = amount; // Używamy zmiennej o typie number, aby uspokoić TypeScript

    this.isLoading = true;
    this.tradeMessage = null;

    const tradeRequest: TradeRequest = {
      fromCurrency,
      toCurrency,
      amount: numericAmount,
    };

    try {
      // Użycie firstValueFrom do poprawnego oczekiwania na wynik Observable z serwisu.
      await firstValueFrom(this.walletService.executeTrade(tradeRequest));
      
      this.tradeMessage = `✅ Sukces! Transakcja ${fromCurrency} -> ${toCurrency} pomyślnie wykonana.`;
      
      // Kluczowa poprawka: Po sukcesie, ładowanie nowego salda, aby odzwierciedlić zmianę na UI.
      await this.initializeBalance(); 

    } catch (error) {
      console.error('Błąd transakcji:', error);
      this.tradeMessage = '❌ Błąd: Nie udało się wykonać transakcji. Sprawdź dostępność środków lub poprawność walut.';
    } finally {
      this.isLoading = false;
    }
  }
}
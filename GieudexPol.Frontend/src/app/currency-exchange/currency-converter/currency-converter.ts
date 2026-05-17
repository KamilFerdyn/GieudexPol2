import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-currency-converter',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './currency-converter.html',
  styleUrls: ['./currency-converter.css']
})
export class CurrencyConverterComponent {
  amount: number = 0;
  sourceCurrency: string = 'PLN';
  targetCurrency: string = 'USD';
  fee: number = 1;

  resultAmount: number | null = null;
  resultFee: number | null = null;
  resultTotal: number | null = null;

  // Kursy walut względem PLN (1 PLN = X waluty)
  private exchangeRates: Record<string, number> = {
    PLN: 1,
    USD: 4.25,
    EUR: 4.50,
    GBP: 5.30,
    JPY: 0.032,
    CHF: 4.60
  };

  get availableCurrencies(): string[] {
    return Object.keys(this.exchangeRates);
  }

  // Funkcja do konwersji kwoty z waluty źródłowej na PLN
  private convertToPLN(amount: number, currency: string): number {
    if (currency === 'PLN') {
      return amount;
    }
    return amount * this.exchangeRates[currency];
  }

  // Funkcja do konwersji kwoty z PLN na walutę docelową
  private convertFromPLN(amountInPLN: number, currency: string): number {
    if (currency === 'PLN') {
      return amountInPLN;
    }
    return amountInPLN / this.exchangeRates[currency];
  }

  // Funkcja do obliczania wymiany walut
  calculateExchange(): void {
    if (this.amount <= 0 || !this.sourceCurrency || !this.targetCurrency) {
      alert('Proszę uzupełnić wszystkie pola poprawnie.');
      return;
    }

    const amountInPLN = this.convertToPLN(this.amount, this.sourceCurrency);
    const exchangedAmount = this.convertFromPLN(amountInPLN, this.targetCurrency);
    const feeAmount = exchangedAmount * (this.fee / 100);
    const totalAmount = exchangedAmount - feeAmount;

    this.resultAmount = exchangedAmount;
    this.resultFee = feeAmount;
    this.resultTotal = totalAmount;
  }
}
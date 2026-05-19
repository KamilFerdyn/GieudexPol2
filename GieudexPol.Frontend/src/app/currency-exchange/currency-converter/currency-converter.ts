import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-currency-converter',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './currency-converter.html',
  styleUrls: ['./currency-converter.css']
})
export class CurrencyConverterComponent {

  constructor(private http: HttpClient) { }

  amount: number = 0;
  sourceCurrency: string = 'PLN';
  targetCurrency: string = 'USD';
  fee: number = 1;

  resultAmount: number | null = null;
  resultFee: number | null = null;
  resultTotal: number | null = null;

  availableCurrencies: string[] = [
    'PLN',
    'USD',
    'EUR',
    'GBP',
    'JPY',
    'CHF'
  ];

  calculateExchange(): void {

    if (this.amount <= 0 || !this.sourceCurrency || !this.targetCurrency) {
      alert('Proszę uzupełnić wszystkie pola poprawnie.');
      return;
    }

    const request = {
      amount: this.amount,
      sourceCurrency: this.sourceCurrency,
      targetCurrency: this.targetCurrency,
      feePercent: this.fee
    };

    this.http.post<any>(
  'http://localhost:5265/api/exchange/calculate',
      request
    )
      .subscribe({
        next: (response) => {

          this.resultAmount = response.convertedAmount;
          this.resultFee = response.feeAmount;
          this.resultTotal = response.finalAmount;

        },

        error: (err) => {
          console.error(err);
          alert(err.error?.message || 'Wystąpił błąd.');        }
      });
  }
}
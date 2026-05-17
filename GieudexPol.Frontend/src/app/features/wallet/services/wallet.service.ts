import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TradeRequest, TradeResponse } from '../models/wallet-models';
@Injectable({
  providedIn: 'root'
})
export class WalletService {
  // Zakładany endpoint API dla transakcji wymiany walut. 
  // Musi być chroniony i wymagać tokena JWT.
  private apiUrl = 'https://localhost:7082/api/wallet';
  constructor(private http: HttpClient) {}
  /**
   * Wykonuje symulowaną transakcję wymiany walut między użytkownikami.
   * @param request Obiekt zawierający waluty i kwotę do wymiany.
   * @returns Observable<TradeResponse> z wynikiem operacji.
   */
  executeTrade(request: TradeRequest): Observable<TradeResponse> {
    // W prawdziwej aplikacji, tutaj musimy dodać nagłówek Authorization: Bearer <token>
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      // Tutaj powinien być dodany token JWT pobrany z AuthService
    });

    console.log('Wykonanie transakcji dla:', request);

    return this.http.post<TradeResponse>(`${this.apiUrl}/trade`, request, { headers });
  }
  /**
   * Pobiera aktualne saldo portfela użytkownika.
   * @returns Promise<{ [key: string]: number }> obiecuje obiekt z walutami i ich wartościami.
   */
  async getBalance(): Promise<{ [key: string]: number }> {
    // Symulacja pobrania danych salda z API. 
    // W prawdziwej aplikacji, tutaj wywołamy GET /api/wallet/balance z tokenem JWT.
    await new Promise(resolve => setTimeout(resolve, 300)); // Symulowany delay API
    return { PLN: Math.floor(Math.random() * 5000) + 1000, EUR: Math.floor(Math.random() * 4000) + 500 };
  }
}
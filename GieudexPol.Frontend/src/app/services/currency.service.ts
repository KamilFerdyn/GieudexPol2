import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CurrencyDto } from '../models/currency.dto';

@Injectable({
  providedIn: 'root'
})
export class CurrencyService {
  private apiUrl = '/api/Currencies'; // Endpoint na podstawie CurrenciesController
  
  constructor(private http: HttpClient) {}

  // GET /api/currencies
  getAllCurrencies(): Observable<CurrencyDto[]> {
    return this.http.get<CurrencyDto[]>(this.apiUrl);
  }

  // GET /api/currencies/{symbol}
  getCurrencyBySymbol(symbol: string): Observable<CurrencyDto> {
    return this.http.get<CurrencyDto>(`${this.apiUrl}/${symbol}`);
  }

  // POST /api/currencies
  createCurrency(currency: CurrencyDto): Observable<CurrencyDto> {
    return this.http.post<CurrencyDto>(this.apiUrl, currency);
  }

  // PUT /api/currencies/{id}
  updateCurrency(currencyId: number, currency: CurrencyDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${currencyId}`, currency);
  }

  // DELETE /api/currencies/{id}
  deleteCurrency(currencyId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${currencyId}`);
  }
}
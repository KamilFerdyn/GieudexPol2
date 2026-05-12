import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ExchangeRateDto } from '../models/exchange-rate.dto';

@Injectable({
  providedIn: 'root'
})
export class ExchangeRateService {
  private apiUrl = '/api/ExchangeRates'; // Endpoint na podstawie ExchangeRatesController
  
  constructor(private http: HttpClient) {}

  // GET /api/exchange-rates/{baseCurrencySymbol}/{targetCurrencySymbol}
  getExchangeRateByPair(baseSymbol: string, targetSymbol: string): Observable<ExchangeRateDto> {
    return this.http.get<ExchangeRateDto>(`${this.apiUrl}/${baseSymbol}/${targetSymbol}`);
  }

  // GET /api/exchange-rates
  getAllExchangeRates(): Observable<ExchangeRateDto[]> {
    return this.http.get<ExchangeRateDto[]>(this.apiUrl);
  }

  // POST /api/exchange-rates
  createExchangeRate(rate: ExchangeRateDto): Observable<ExchangeRateDto> {
    return this.http.post<ExchangeRateDto>(this.apiUrl, rate);
  }

  // PUT /api/exchange-rates/{id}
  updateExchangeRate(rateId: number, rate: ExchangeRateDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${rateId}`, rate);
  }

  // DELETE /api/exchange-rates/{id}
  deleteExchangeRate(rateId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${rateId}`);
  }
}
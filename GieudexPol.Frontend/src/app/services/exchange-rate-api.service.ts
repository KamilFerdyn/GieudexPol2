import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  ExchangeRateChartResponse,
  ExchangeRateTableRow,
} from '../models/exchange-rate.models';

@Injectable({
  providedIn: 'root',
})
export class ExchangeRateApiService {
  private readonly apiUrl = '/api/ExchangeRates';

  constructor(private readonly http: HttpClient) {}

  getChartData(
    currency: string,
    source: string,
    from: string,
    to: string,
  ): Observable<ExchangeRateChartResponse> {
    const params = new HttpParams()
      .set('currency', currency)
      .set('source', source)
      .set('from', from)
      .set('to', to);

    return this.http.get<ExchangeRateChartResponse>(`${this.apiUrl}/chart`, { params });
  }

  getLatestRates(source: string): Observable<ExchangeRateTableRow[]> {
    const params = new HttpParams().set('source', source);

    return this.http.get<ExchangeRateTableRow[]>(`${this.apiUrl}/latest`, { params });
  }
}

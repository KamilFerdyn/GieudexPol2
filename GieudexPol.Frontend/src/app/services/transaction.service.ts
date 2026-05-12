import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TransactionDto } from '../models/transaction.dto';

@Injectable({
  providedIn: 'root'
})
export class TransactionService {
  private apiUrl = '/api/Transactions'; // Endpoint na podstawie TransactionsController
  
  constructor(private http: HttpClient) {}

  // GET /api/transactions/user/{userId}
  getUserTransactions(userId: number): Observable<TransactionDto[]> {
    return this.http.get<TransactionDto[]>(`${this.apiUrl}/user/${userId}`);
  }

  // POST /api/transactions
  createTransaction(transaction: TransactionDto): Observable<TransactionDto> {
    return this.http.post<TransactionDto>(this.apiUrl, transaction);
  }
}
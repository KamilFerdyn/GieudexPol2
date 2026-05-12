import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { WalletDto } from '../models/wallet.dto';

@Injectable({
  providedIn: 'root'
})
export class WalletService {
  private apiUrl = '/api/Wallets'; // Endpoint na podstawie WalletsController
  
  constructor(private http: HttpClient) {}

  // GET /api/wallets/user/{userId}
  getUserWallets(userId: number): Observable<WalletDto[]> {
    return this.http.get<WalletDto[]>(`${this.apiUrl}/user/${userId}`);
  }

  // GET /api/wallets/{id}
  getWalletById(walletId: number): Observable<WalletDto> {
    return this.http.get<WalletDto>(`${this.apiUrl}/${walletId}`);
  }

  // POST /api/wallets
  createWallet(wallet: WalletDto): Observable<WalletDto> {
    return this.http.post<WalletDto>(this.apiUrl, wallet);
  }

  // PUT /api/wallets/{id}
  updateWallet(walletId: number, wallet: WalletDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${walletId}`, wallet);
  }

  // DELETE /api/wallets/{id}
  deleteWallet(walletId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${walletId}`);
  }
}
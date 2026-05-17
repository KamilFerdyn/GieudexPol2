/**
 * Definicje typów dla modułu zarządzania portfelem.
 */

export interface WalletBalance {
  currencyCode: string; // np. PLN, EUR
  balance: number;
}

export interface TradeRequest {
  fromCurrency: string;
  toCurrency: string;
  amount: number;
}

export interface TradeResponse {
  success: boolean;
  message: string;
  newBalance?: WalletBalance; // Opcjonalne, jeśli transakcja powiodła się i zaktualizowano saldo.
}
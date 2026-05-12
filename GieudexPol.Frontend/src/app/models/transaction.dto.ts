export interface TransactionDto {
  transactionId: number;
  senderUserId: number;
  receiverUserId: number;
  amount: number;
  currencySymbol: string;
  exchangeRateUsed: number | null; // Może być null, jeśli transakcja w tej samej walucie co sender.
  timestamp: Date;
}
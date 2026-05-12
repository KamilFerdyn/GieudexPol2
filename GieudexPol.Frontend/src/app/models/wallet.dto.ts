export interface WalletDto {
  walletId: number;
  ownerUserId: number;
  balance: number;
  currencySymbol: string;
}

// DTO do tworzenia portfela, jeśli wymagane przez logikę biznesową.
export interface CreateWalletDto {
    ownerUserId: number;
}
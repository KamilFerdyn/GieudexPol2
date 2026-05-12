export interface ExchangeRateDto {
  id: number;
  baseCurrencySymbol: string;
  targetCurrencySymbol: string;
  rate: number;
  lastUpdated: Date;
}

// Dodatkowe DTO dla operacji PUT/POST, jeśli wymagane przez logikę biznesową.
export interface ExchangeRateUpdateDto {
    rate: number;
    baseCurrencySymbol: string;
    targetCurrencySymbol: string;
}
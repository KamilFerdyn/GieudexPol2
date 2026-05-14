export interface ExchangeRateChartPoint {
  date: string;
  buyPrice: number;
  sellPrice: number;
}

export interface ExchangeRateChartResponse {
  currencyCode: string;
  sourceCode: string;
  from: string;
  to: string;
  points: ExchangeRateChartPoint[];
}

export interface ExchangeRateTableRow {
  currencyCode: string;
  currencyName: string;
  sourceCode: string;
  sourceName: string;
  effectiveDate: string;
  buyPrice: number;
  sellPrice: number;
}

import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { timeout } from 'rxjs/operators';
import {
  ExchangeRateChartPoint,
  ExchangeRateTableRow,
} from '../models/exchange-rate.models';
import { ExchangeRateApiService } from '../services/exchange-rate-api.service';

@Component({
  selector: 'app-exchange-rate-dashboard',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './exchange-rate-dashboard.html',
  styleUrl: './exchange-rate-dashboard.css',
})
export class ExchangeRateDashboard implements OnInit {
  readonly currencies = ['EUR', 'USD', 'CHF', 'GBP'];
  readonly sources = [
    {
      code: 'MOCK_BANK_A',
      label: 'MOCK_BANK_A - fallback dev',
    },
  ];
  readonly rangePresets = [
    { label: '7D', days: 7 },
    { label: '30D', days: 30 },
    { label: '90D', days: 90 },
    { label: 'YTD', ytd: true },
    { label: 'DEV', from: '2026-01-01' },
  ];
  readonly chartWidth = 920;
  readonly chartHeight = 320;
  readonly chartPadding = {
    top: 24,
    right: 26,
    bottom: 44,
    left: 58,
  };

  currency = 'EUR';
  source = 'MOCK_BANK_A';
  from = '2026-01-01';
  to = this.formatDateInput(new Date());

  chartPoints: ExchangeRateChartPoint[] = [];
  latestRates: ExchangeRateTableRow[] = [];
  loading = false;
  errorMessage = '';
  statusMessage = 'Gotowe do pobrania danych.';
  lastLoadedAt: Date | null = null;
  selectedPointIndex: number | null = null;

  constructor(
    private readonly exchangeRateApi: ExchangeRateApiService,
    private readonly changeDetector: ChangeDetectorRef,
  ) {}

  ngOnInit(): void {
    this.fetchData();
  }

  async fetchData(): Promise<void> {
    if (this.loading) {
      return;
    }

    this.errorMessage = '';
    this.statusMessage = 'Pobieram dane z backendu...';
    this.selectedPointIndex = null;

    if (!this.currency || !this.source || !this.from || !this.to) {
      this.errorMessage = 'Uzupelnij walute, zrodlo i zakres dat.';
      this.statusMessage = 'Wymagane sa wszystkie filtry.';
      return;
    }

    if (this.from > this.to) {
      this.errorMessage = 'Data poczatkowa nie moze byc pozniejsza niz koncowa.';
      this.statusMessage = 'Zakres dat wymaga poprawy.';
      return;
    }

    this.loading = true;

    try {
      const [chartResult, latestResult] = await Promise.allSettled([
        firstValueFrom(
          this.exchangeRateApi
            .getChartData(this.currency, this.source, this.from, this.to)
            .pipe(timeout(15000)),
        ),
        firstValueFrom(
          this.exchangeRateApi
            .getLatestRates(this.source)
            .pipe(timeout(15000)),
        ),
      ]);

      if (chartResult.status === 'fulfilled') {
        this.chartPoints = chartResult.value.points ?? [];
      } else {
        this.chartPoints = [];
        this.errorMessage = 'Nie udalo sie pobrac danych wykresu. Sprawdz backend albo zakres dat.';
      }

      if (latestResult.status === 'fulfilled') {
        this.latestRates = latestResult.value;
      } else {
        this.latestRates = [];
        this.errorMessage = this.errorMessage
          ? `${this.errorMessage} Nie udalo sie pobrac tabeli najnowszych kursow.`
          : 'Nie udalo sie pobrac tabeli najnowszych kursow.';
      }

      if (!this.errorMessage) {
        this.statusMessage = this.chartPoints.length
          ? `Gotowe. Wczytano ${this.chartPoints.length} dni kursowych.`
          : 'Gotowe, ale backend nie zwrocil danych dla wybranych filtrow.';
      } else {
        this.statusMessage = 'Pobieranie zakonczone bledem.';
      }

      this.lastLoadedAt = new Date();
    } finally {
      this.loading = false;
      this.changeDetector.detectChanges();
    }
  }

  get latestPoint(): ExchangeRateChartPoint | null {
    return this.chartPoints.at(-1) ?? null;
  }

  get latestSelectedRate(): ExchangeRateTableRow | null {
    return this.latestRates.find((rate) => rate.currencyCode === this.currency) ?? null;
  }

  get selectedPoint(): ExchangeRateChartPoint | null {
    if (this.selectedPointIndex === null) {
      return null;
    }

    return this.chartPoints[this.selectedPointIndex] ?? null;
  }

  spread(point: ExchangeRateChartPoint): number {
    return point.sellPrice - point.buyPrice;
  }

  get chartMinValue(): number {
    const values = this.chartValues;
    return values.length ? Math.min(...values) : 0;
  }

  get chartMaxValue(): number {
    const values = this.chartValues;
    return values.length ? Math.max(...values) : 1;
  }

  get chartRange(): number {
    const range = this.chartMaxValue - this.chartMinValue;
    return range === 0 ? 1 : range;
  }

  get gridLines(): Array<{ y: number; label: number }> {
    const lines = 4;

    return Array.from({ length: lines + 1 }, (_, index) => {
      const ratio = index / lines;
      const label = this.chartMaxValue - this.chartRange * ratio;

      return {
        y: this.chartPadding.top + this.plotHeight * ratio,
        label,
      };
    });
  }

  get buyLinePoints(): string {
    return this.createLinePoints('buyPrice');
  }

  get sellLinePoints(): string {
    return this.createLinePoints('sellPrice');
  }

  get firstPointDate(): string {
    return this.chartPoints[0]?.date ?? '';
  }

  get lastPointDate(): string {
    return this.chartPoints.at(-1)?.date ?? '';
  }

  get selectedPointX(): number {
    return this.selectedPointIndex === null ? 0 : this.getPointX(this.selectedPointIndex);
  }

  get selectedBuyY(): number {
    const point = this.selectedPoint;
    return point ? this.getPointY(point.buyPrice) : 0;
  }

  get selectedSellY(): number {
    const point = this.selectedPoint;
    return point ? this.getPointY(point.sellPrice) : 0;
  }

  get selectedTooltipX(): number {
    return Math.min(Math.max(this.selectedPointX - 70, this.chartPadding.left), this.chartWidth - 190);
  }

  get selectedTooltipY(): number {
    return Math.max(Math.min(Math.min(this.selectedBuyY, this.selectedSellY) - 88, this.chartHeight - 128), 12);
  }

  get plotWidth(): number {
    return this.chartWidth - this.chartPadding.left - this.chartPadding.right;
  }

  get plotHeight(): number {
    return this.chartHeight - this.chartPadding.top - this.chartPadding.bottom;
  }

  trackByDate(_: number, point: ExchangeRateChartPoint): string {
    return point.date;
  }

  trackByCurrency(_: number, rate: ExchangeRateTableRow): string {
    return `${rate.sourceCode}-${rate.currencyCode}`;
  }

  selectChartPoint(event: MouseEvent): void {
    if (this.chartPoints.length === 0) {
      this.selectedPointIndex = null;
      return;
    }

    const target = event.currentTarget as SVGSVGElement;
    const bounds = target.getBoundingClientRect();
    const ratio = (event.clientX - bounds.left) / bounds.width;
    const x = ratio * this.chartWidth;
    const clampedX = Math.min(
      Math.max(x, this.chartPadding.left),
      this.chartWidth - this.chartPadding.right,
    );
    const plotRatio = (clampedX - this.chartPadding.left) / this.plotWidth;
    const index = Math.round(plotRatio * (this.chartPoints.length - 1));

    this.selectedPointIndex = Math.min(Math.max(index, 0), this.chartPoints.length - 1);
  }

  clearSelectedPoint(): void {
    this.selectedPointIndex = null;
  }

  selectCurrency(currency: string): void {
    this.currency = currency;
    void this.fetchData();
  }

  applyRangePreset(preset: { days?: number; ytd?: boolean; from?: string }): void {
    const today = new Date();
    this.to = this.formatDateInput(today);

    if (preset.from) {
      this.from = preset.from;
    } else if (preset.ytd) {
      this.from = `${today.getFullYear()}-01-01`;
    } else if (preset.days) {
      const startDate = new Date(today);
      startDate.setDate(startDate.getDate() - preset.days);
      this.from = this.formatDateInput(startDate);
    }

    void this.fetchData();
  }

  private formatDateInput(date: Date): string {
    return date.toISOString().slice(0, 10);
  }

  private get chartValues(): number[] {
    return this.chartPoints.flatMap((point) => [point.buyPrice, point.sellPrice]);
  }

  private createLinePoints(field: 'buyPrice' | 'sellPrice'): string {
    if (this.chartPoints.length === 0) {
      return '';
    }

    const lastIndex = Math.max(this.chartPoints.length - 1, 1);

    return this.chartPoints
      .map((point, index) => {
        const x = this.getPointX(index);
        const y = this.getPointY(point[field]);

        return `${x.toFixed(2)},${y.toFixed(2)}`;
      })
      .join(' ');
  }

  private getPointX(index: number): number {
    const lastIndex = Math.max(this.chartPoints.length - 1, 1);
    return this.chartPadding.left + (this.plotWidth * index) / lastIndex;
  }

  private getPointY(value: number): number {
    const normalized = (value - this.chartMinValue) / this.chartRange;
    return this.chartPadding.top + this.plotHeight - normalized * this.plotHeight;
  }

}

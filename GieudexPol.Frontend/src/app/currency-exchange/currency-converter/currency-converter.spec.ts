import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CurrencyConverter } from './currency-converter';

describe('CurrencyConverter', () => {
  let component: CurrencyConverter;
  let fixture: ComponentFixture<CurrencyConverter>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CurrencyConverter],
    }).compileComponents();

    fixture = TestBed.createComponent(CurrencyConverter);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

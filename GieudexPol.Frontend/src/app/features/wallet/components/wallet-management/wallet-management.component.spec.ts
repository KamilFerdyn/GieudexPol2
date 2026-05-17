import { ComponentFixture, TestBed } from '@angular/core/testing';
import { WalletManagementComponent } from './wallet-management.component';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AuthService } from '../services/auth.service'; // Możliwe poleganie na AuthService w przyszłości
import { WalletService } from '../services/wallet.service';
import { By } from '@angular/platform-browser';
import { of, throwError } from 'rxjs';

describe('WalletManagementComponent', () => {
  let component: WalletManagementComponent;
  let fixture: ComponentFixture<WalletManagementComponent>;
  let mockWalletService: jasmine.SpyObj<WalletService>;
  let httpMock: HttpTestingController;

  beforeEach(async(() => {
    // Mockowanie usługi WalletService
    mockWalletService = jasmine.createSpyObj('WalletService', ['executeTrade']);

    TestBed.configureTestingModule({
      declarations: [WalletManagementComponent],
      providers: [
        { provide: WalletService, useValue: mockWalletService }
      ],
      imports: [
        // Importowanie modułów potrzebnych do testów (np. FormsModule dla ngModel)
        // W rzeczywistym projekcie może być konieczne dodanie NgModule z formami.
      ],
      schemas: [
        // Możliwe, że trzeba będzie dodać Schemas dla formularzy Angular
      ]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WalletManagementComponent);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
    // Mockowanie inicjalizacji salda, aby testy były czyste
    spyOn(component as any, 'initializeBalance').and.callThrough();
    fixture.detectChanges(); // Wywołanie ngOnInit
  });

  it('powinien zainicjalizować saldo i ustawić stan początkowy', () => {
    expect(component).toBeTruthy();
    // Sprawdzenie, czy komponent załadował domyślne salda po wywołaniu initializeBalance
    const balanceLi = fixture.debugElement.query(By.css('.balance-summary li')).toArray();
    expect(balanceLi.length).toBe(3); // PLN, EUR, USD
  });

  describe('executeTrade', () => {
    let tradeSpy: jasmine.Spy;

    beforeEach(() => {
      // Spy na metodzie executeTrade w mockowanym serwisie
      tradeSpy = spyOn(mockWalletService, 'executeTrade').and.returnValue(of({ success: true, message: 'OK' } as any));
    });

    it('powinien wywołać API z poprawnymi danymi przy udanej próbie wymiany', async () => {
      // Symulacja interakcji użytkownika
      component['fromCurrency'] = { value: 'PLN' };
      component['toCurrency'] = { value: 'EUR' };
      component['amount'] = 100;

      await component.executeTrade('PLN', 'EUR', 100);

      // Sprawdzenie wywołania serwisu
      expect(tradeSpy).toHaveBeenCalledWith({
        fromCurrency: 'PLN',
        toCurrency: 'EUR',
        amount: 100,
      });
    });

    it('powinien ustawić komunikat sukcesu po pomyślnym wywołaniu API', async () => {
      component['fromCurrency'] = { value: 'PLN' };
      component['toCurrency'] = { value: 'EUR' };
      component['amount'] = 100;

      await component.executeTrade('PLN', 'EUR', 100);

      // Sprawdzenie, czy komunikat jest ustawiony i ma poprawny format sukcesu
      expect(component.tradeMessage).toContain('Sukces!');
    });

    it('powinien obsłużyć błąd API i wyświetlić komunikat o błędzie', async () => {
      const mockError = new ErrorEvent('Network Error');
      // Mockowanie serwisu tak, aby zwrócił błąd
      tradeSpy.and.returnValue(throwError(() => mockError));

      component['fromCurrency'] = { value: 'PLN' };
      component['toCurrency'] = { value: 'EUR' };
      component['amount'] = 100;

      await component.executeTrade('PLN', 'EUR', 100);

      // Sprawdzenie, czy komunikat jest ustawiony i ma poprawny format błędu
      expect(component.tradeMessage).toContain('Błąd');
    });

    it('powinien blokować przyciski i nie wywoływać API dla nieważnych danych (np. kwota 0)', async () => {
        // Ustawienie stanów, które powinny spowodować błąd walidacyjny na froncie
        component['fromCurrency'] = { value: 'PLN' };
        component['toCurrency'] = { value: 'EUR' };
        component['amount'] = 0;

        await component.executeTrade('PLN', 'EUR', 0);

        // Sprawdzenie, że API nie zostało wywołane i wyświetlono komunikat walidacyjny
        expect(tradeSpy).not.toHaveBeenCalled();
        expect(component.tradeMessage).toContain('Kwota musi być większa od zera');
    });
  });
});
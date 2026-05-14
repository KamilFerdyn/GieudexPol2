# Specyfikacja funkcjonalnosci: pobieranie kursow walut z NBP

## 1. Cel funkcjonalnosci

Celem funkcjonalnosci jest pobieranie rzeczywistych kursow kupna i sprzedazy walut z zewnetrznego zrodla NBP oraz zapisanie ich w lokalnej bazie danych aplikacji GieudexPol.

Funkcjonalnosc pozwala frontendowi wyswietlac dane historyczne na wykresie oraz w tabelach bez bezposredniego komunikowania sie z API NBP. Frontend korzysta tylko z backendu aplikacji.

Aktualnie obslugiwane zrodlo zewnetrzne:

- `NBP` - Narodowy Bank Polski

Dodatkowo w trybie developerskim pozostaje fallback:

- `MOCK_BANK_A` - lokalne dane seedowane w srodowisku Development

## 2. Zakres funkcjonalnosci

Funkcjonalnosc obejmuje:

- pobieranie kursow z API NBP,
- korzystanie wylacznie z tabeli `C`, poniewaz zawiera kurs kupna `bid` i kurs sprzedazy `ask`,
- dzielenie zakresu dat na paczki maksymalnie po 93 dni,
- tworzenie zrodla kursu `NBP`, jezeli nie istnieje,
- tworzenie walut, jezeli nie istnieja lokalnie,
- zapisywanie kursow kupna i sprzedazy do tabeli `ExchangeRates`,
- pomijanie duplikatow dla tej samej waluty, zrodla i daty,
- udostepnienie endpointu backendowego do recznej synchronizacji,
- automatyczne uruchomienie synchronizacji przez frontend, gdy uzytkownik wybierze zrodlo `NBP`, a lokalnie nie ma danych dla wybranego zakresu.

Funkcjonalnosc nie obejmuje:

- automatycznej synchronizacji przy starcie aplikacji,
- `BackgroundService`,
- pobierania kursu sredniego `mid`,
- obslugi tabeli `A` lub `B`,
- zapisu nazwy tabeli NBP w encji kursu,
- bezposredniego wolania API NBP z frontendu.

## 3. Aktorzy i systemy

Glowni uczestnicy procesu:

- Uzytkownik - wybiera walute, zrodlo i zakres dat w widoku `/rates`.
- Frontend Angular - sprawdza lokalne dane i inicjuje synchronizacje, jezeli trzeba.
- Backend ASP.NET Core - udostepnia endpointy odczytu i synchronizacji.
- Serwis synchronizacji - odpowiada za logike pobierania i zapisu danych NBP.
- API NBP - zewnetrzne zrodlo danych kursowych.
- Baza danych - przechowuje waluty, zrodla kursow i kursy historyczne.

## 4. Przeplyw podstawowy

1. Uzytkownik wchodzi na strone `/rates`.
2. Uzytkownik wybiera zrodlo `NBP`.
3. Frontend wysyla zapytanie do backendu o dane lokalne:

```http
GET /api/ExchangeRates/chart?currency=EUR&source=NBP&from=2026-01-01&to=2026-05-14
```

4. Backend sprawdza lokalna baze danych.
5. Jezeli dane istnieja, backend zwraca DTO z punktami wykresu.
6. Frontend wyswietla wykres i tabele.

## 5. Przeplyw alternatywny: brak lokalnych danych NBP

1. Uzytkownik wybiera zrodlo `NBP`.
2. Frontend odpytuje backend o lokalne dane.
3. Backend zwraca odpowiedz z pusta lista punktow.
4. Frontend automatycznie uruchamia synchronizacje:

```http
POST /api/ExchangeRates/sync/nbp?from=2026-01-01&to=2026-05-14
```

5. Backend waliduje zakres dat.
6. Serwis synchronizacji znajduje lub tworzy `RateSource`:

```text
Code = NBP
Name = Narodowy Bank Polski
IsActive = true
```

7. Serwis dzieli zakres dat na fragmenty maksymalnie po 93 dni.
8. Dla kazdego fragmentu wywolywane jest API NBP:

```http
GET https://api.nbp.pl/api/exchangerates/tables/C/{from}/{to}/?format=json
```

9. Dla kazdej tabeli i kazdej waluty:

- `rate.code` jest mapowane na `Currency.Symbol`,
- `rate.currency` jest mapowane na `Currency.Name`,
- `rate.bid` jest mapowane na `ExchangeRate.BuyPrice`,
- `rate.ask` jest mapowane na `ExchangeRate.SellPrice`,
- `table.effectiveDate` jest mapowane na `ExchangeRate.EffectiveDate`,
- `DateTime.UtcNow` jest mapowane na `ExchangeRate.FetchedAt`.

10. Jezeli waluta nie istnieje, zostaje dodana.
11. Jezeli kurs dla kombinacji `CurrencyId + RateSourceId + EffectiveDate` juz istnieje, zostaje pominiety.
12. Jezeli kurs nie istnieje, zostaje dodany.
13. Backend zapisuje zmiany w bazie.
14. Backend zwraca `NbpSyncResultDto`.
15. Frontend ponownie pobiera dane lokalne i wyswietla wykres oraz tabele.

## 6. Walidacje i ograniczenia

Endpoint synchronizacji:

```http
POST /api/ExchangeRates/sync/nbp
```

przyjmuje parametry:

- `from` - data poczatkowa,
- `to` - data koncowa.

Walidacje:

- jezeli `from > to`, backend zwraca `400 Bad Request`,
- jezeli `from < 2026-01-01`, backend zwraca `400 Bad Request`,
- jezeli `to > DateTime.Today`, backend zwraca `400 Bad Request`,
- zakres wysylany do NBP jest dzielony na paczki maksymalnie po 93 dni,
- jezeli NBP zwroci `404` dla zakresu bez danych, klient NBP zwraca pusta liste zamiast przerywac cala synchronizacje,
- jezeli NBP zwroci `400`, backend traktuje to jako blad niepoprawnego zakresu lub zapytania.

## 7. Model danych

Funkcjonalnosc korzysta z encji:

### Currency

Reprezentuje walute, np. `EUR`, `USD`, `CHF`.

Najwazniejsze pola:

- `Id`,
- `Symbol`,
- `Name`,
- `IsActive`,
- `ExchangeRates`.

### RateSource

Reprezentuje zrodlo kursu, np. `NBP` albo `MOCK_BANK_A`.

Najwazniejsze pola:

- `Id`,
- `Code`,
- `Name`,
- `IsActive`,
- `ExchangeRates`.

### ExchangeRate

Reprezentuje kurs waluty z konkretnego zrodla dla konkretnej daty.

Najwazniejsze pola:

- `Id`,
- `CurrencyId`,
- `Currency`,
- `RateSourceId`,
- `RateSource`,
- `BuyPrice`,
- `SellPrice`,
- `EffectiveDate`,
- `FetchedAt`.

Dla `ExchangeRate` ustawiono unikalnosc:

```text
CurrencyId + RateSourceId + EffectiveDate
```

Dzieki temu ten sam kurs tej samej waluty z tego samego zrodla dla tej samej daty nie zostanie zapisany wielokrotnie.

## 8. Warstwy i klasy

### Frontend

- `ExchangeRateDashboard`
  - widok `/rates`,
  - pozwala wybrac walute, zrodlo i zakres dat,
  - po wyborze `NBP` najpierw sprawdza lokalne dane,
  - jezeli lokalnych danych nie ma, uruchamia synchronizacje NBP.

- `ExchangeRateApiService`
  - wywoluje endpointy backendu:
    - `GET /api/ExchangeRates/chart`,
    - `GET /api/ExchangeRates/latest`,
    - `POST /api/ExchangeRates/sync/nbp`.

### API

- `ExchangeRatesController`
  - udostepnia endpointy odczytu kursow,
  - udostepnia endpoint synchronizacji NBP,
  - wykonuje podstawowa walidacje zakresu dat.

### Application

- `IExchangeRateSyncService`
  - interfejs logiki synchronizacji kursow z NBP.

- `INbpExchangeRateClient`
  - interfejs klienta HTTP do API NBP.

- DTO:
  - `NbpExchangeRateTableDto`,
  - `NbpExchangeRateItemDto`,
  - `NbpSyncResultDto`,
  - `ExchangeRateChartResponseDto`,
  - `ExchangeRateChartPointDto`,
  - `ExchangeRateTableRowDto`.

### Infrastructure

- `ExchangeRateSyncService`
  - glowna logika synchronizacji,
  - dzieli zakres dat na paczki po 93 dni,
  - pobiera dane z NBP,
  - tworzy waluty i zrodlo NBP,
  - zapisuje nowe kursy,
  - pomija duplikaty.

- `NbpExchangeRateClient`
  - komunikuje sie z API NBP przez `HttpClient`,
  - pobiera JSON z tabeli `C`,
  - mapuje odpowiedz na DTO.

- `ApplicationDbContext`
  - udostepnia `DbSet<Currency>`,
  - udostepnia `DbSet<RateSource>`,
  - udostepnia `DbSet<ExchangeRate>`,
  - konfiguruje relacje i indeks unikalny.

## 9. Endpointy

### Synchronizacja NBP

```http
POST /api/ExchangeRates/sync/nbp?from=2026-01-01&to=2026-05-14
```

Zwraca:

```text
NbpSyncResultDto
```

Najwazniejsze pola odpowiedzi:

- `From`,
- `To`,
- `Added`,
- `Skipped`,
- `TablesFetched`,
- `ProcessedRanges`,
- `Warnings`.

### Dane do wykresu

```http
GET /api/ExchangeRates/chart?currency=EUR&source=NBP&from=2026-01-01&to=2026-05-14
```

Zwraca:

```text
ExchangeRateChartResponseDto
```

### Najnowsze kursy

```http
GET /api/ExchangeRates/latest?source=NBP
```

Zwraca:

```text
List<ExchangeRateTableRowDto>
```

## 10. Diagramy

Diagramy PlantUML dla tej funkcjonalnosci:

- `UML/NBP_SyncSequence.puml` - diagram sekwencji,
- `UML/NBP_SyncClassDiagram.puml` - diagram klas.

## 11. Status realizacji wymagan

1. Uzupełnia specyfikację w zakresie precyzyjnego opisu swojej funkcjonalności
   - Zrealizowane w tym dokumencie.

2. Tworzy lub uzupełnia diagramy sekwencji i klas w zakresie swojej funkcjonalności
   - Zrealizowane w plikach PlantUML:
     - `UML/NBP_SyncSequence.puml`,
     - `UML/NBP_SyncClassDiagram.puml`.

3. Uzupełnia dla swojej funkcjonalności klasy modelu domenowego oraz klasy z warstwy dostępu do danych
   - Zrealizowane przez encje `Currency`, `RateSource`, `ExchangeRate`, konfiguracje `ApplicationDbContext` oraz repozytoria kursow i zrodel kursow.

4. Tworzy dla swojej funkcjonalności klasy logiki biznesowej wraz z testami jednostkowymi
   - Czesciowo zrealizowane.
   - Klasy logiki biznesowej istnieja: `ExchangeRateSyncService`.
   - Do uzupelnienia pozostaja dedykowane testy jednostkowe dla synchronizacji NBP.

5. Tworzy kontrolery udostępniające funkcjonalności jego klas biznesowych
   - Zrealizowane przez `ExchangeRatesController` i endpoint `POST /api/ExchangeRates/sync/nbp`.

6. Uzupełnia frontend o obsługę funkcjonalności z jego klas biznesowych
   - Zrealizowane w widoku `/rates`.
   - Frontend po wyborze zrodla `NBP` sprawdza dane lokalne i automatycznie uruchamia synchronizacje, jezeli dane nie istnieja.

## 12. Elementy do dalszego rozwoju

Najwazniejszym brakujacym elementem sa testy jednostkowe dla synchronizacji NBP.

Przykladowe scenariusze testowe:

- synchronizacja tworzy `RateSource` NBP, jezeli nie istnieje,
- synchronizacja tworzy walute, jezeli nie istnieje,
- synchronizacja dodaje nowe kursy z `bid` i `ask`,
- synchronizacja pomija duplikaty,
- synchronizacja dzieli zakres na paczki po maksymalnie 93 dni,
- synchronizacja poprawnie obsluguje pusta odpowiedz NBP,
- endpoint zwraca `400 Bad Request` dla niepoprawnych zakresow dat.

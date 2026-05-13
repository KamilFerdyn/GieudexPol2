# 💹 GieudexPol

Adres strony:
https://gieudexpol2-production.up.railway.app

*wersja "pierwotna do edytkowania" przez pozostałych użytkownikow*

> **Kryptonim projektowy:** `Waluty robią brrrrrrr`  
> **Status:** W fazie implementacji (v1.0)  
> **Cel:** Maksymalizacja zysku z wahań rynkowych poprzez zaawansowaną analitykę danych Live.

---




## 💡 Idea systemu
GieudexPol to silnik, który automatycznie pobiera kursy walut z różnych źródeł (np. NBP, API komercyjne), nakłada na nie własną marżę i szuka różnic w cenach (arbitrażu). System działa jak inteligentny filtr: zamiast przeglądać dziesiątki tabel, użytkownik dostaje gotowe zestawienie par walutowych, na których w danej sekundzie można zarobić.

---



## 🎯 Cel projektu
Agregacja danych: Automatyczne ściąganie kursów z wielu API bez konieczności ręcznego sprawdzania stron banków.

Silnik marżowy: Stworzenie modułu, który w czasie rzeczywistym dolicza prowizje do kursów bazowych.

Detekcja anomalii: Zaprogramowanie algorytmu, który wyłapuje błędy cenowe lub nagłe skoki kursów.

Szybkość powiadomień: Skrócenie czasu od wystąpienia okazji rynkowej do poinformowania o tym użytkownika.


---



## 🛠️ Stack Technologiczny
| Warstwa | Technologia | Opis |
| :--- | :--- | :--- |
| **Frontend** | Angular 17+ | Reaktywny interfejs spekulanta |
| **Backend** | .NET 8 Core | Wydajne WebAPI w architekturze N-Tier |
| **Baza Danych** | MS SQL Server | Relacyjny magazyn danych i historii kursów |
| **AI Agent** | **Cline** (Gemini-2.5-flash) | Autonomiczny agent (VS Code) wspomagający rozwój i audyt kodu |
| **Dokumentacja** | PlantUML | Diagramy architektury i przepływu danych |

---

## 📋 Specyfikacja Systemu (Zadania Indywidualne)

### 1. Analiza i wybór agenta AI
W procesie tworzenia systemu wykorzystano agenta **Cline** działającego w środowisku VS Code. 
* **Model:** Gemini-2.5-flash (wybrany ze względu na wysoką precyzję w logice finansowej).
* **Metodologia pracy:** **Agentic Loop (Pętla Agencyjna)**. Agent nie tylko generuje kod, ale posiada uprawnienia do zarządzania strukturą plików, uruchamiania terminala (CLI) oraz debugowania błędów kompilacji w czasie rzeczywistym.

### 2. Wybór architektury
System oparty jest na wzorcu **Clean Architecture**, co zapewnia separację logiki biznesowej od infrastruktury:
* **GieudexPol.Domain:** Encje (`Currency`, `Rate`, `Alert`) oraz reguły biznesowe.
* **GieudexPol.Application (BLL):** Interfejsy serwisów, logika obliczeń marżowych i system powiadomień.
* **GieudexPol.Infrastructure (DAL):** Implementacja Entity Framework Core, migracje MS SQL oraz integracja z zewnętrznymi API bankowymi.
* **GieudexPol.API:** Kontrolery REST stanowiące punkt styku dla aplikacji Angular.

### 3. Funkcjonalności (Scope)
Funkcjonalności Użytkownika:
- Rejestracja i logowanie: Bezpieczny dostęp do konta z opcjonalną weryfikacją dwuskładnikową (2FA).

- Portfel cyfrowy: Przejrzysty podgląd salda oraz zarządzanie środkami dostępnymi do handlu.

- Składanie zleceń: Intuicyjny formularz kupna i sprzedaży aktywów po cenie rynkowej.

- Arkusz zleceń (Orderbook): Podgląd wszystkich aktywnych ofert innych użytkowników w czasie rzeczywistym.

- Interaktywne wykresy: Analiza trendów cenowych za pomocą profesjonalnych narzędzi wizualnych.

- Historia transakcji: Pełny wgląd w archiwalne operacje, wpłaty oraz zrealizowane zlecenia.

- Alerty cenowe: System powiadomień informujący o osiągnięciu przez aktywo wyznaczonej ceny.

Funkcjonalności Administratora:
- Zarządzanie użytkownikami: Pełna baza profili z opcją blokowania, usuwania i resetowania haseł.

- Konfiguracja prowizji: Możliwość globalnej zmiany procentowych opłat za każdą transakcję na giełdzie.

- Zarządzanie rynkami: Dodawanie nowych par handlowych (np. BTC/PLN) oraz wstrzymywanie handlu w sytuacjach awaryjnych.

- Monitoring bezpieczeństwa: Podgląd logów systemowych i wykrywanie podejrzanych aktywności lub prób włamań.

- Raporty finansowe: Generowanie zestawień dotyczących wolumenu obrotów i całkowitego zarobku platformy.

---

## 🗄️ Struktura Bazy Danych
Kluczowe encje zarządzane przez Entity Framework:

- Użytkownicy i Bezpieczeństwo:
Users: Przechowuje dane profilowe, skróty haseł (BCrypt/Identity) oraz role (Admin/User).
Wallets: Reprezentuje stan posiadania konkretnej waluty przez użytkownika (np. portfel PLN, portfel BTC).

- Rynek i Kursy:
Currencies: Definicje aktywów (Symbol, Nazwa, Status aktywności).
ExchangeRates: Historia kursów (ceny kupna/sprzedaży) z dokładnością decimal(18,4).

- Operacje i Automatyzacja:
Transactions: Nieusuwalny rejestr wszystkich operacji (kupno/sprzedaż) wraz z naliczoną prowizją.
UserAlerts: Konfiguracja powiadomień – wiąże użytkownika z progiem cenowym danej waluty.
---




## 🚀 **Wdrożenie na Railway.app**

Aplikacja została wdrożona na platformie Railway.app i jest dostępna pod adresem:
👉 [https://gieudexpol2-production.up.railway.app](https://gieudexpol2-production.up.railway.app)


## 🏗️ **Architektura Clean Architecture**

Projekt oparty jest na wzorcu **Clean Architecture**, który zapewnia separację logiki biznesowej od infrastruktury:

```
┌───────────────────────────────────────────────────────────────┐
│                        Client (Frontend)                      │
└───────────────────────────────────────────────────────────────┘
                                 │
                                 ▼
┌───────────────────────────────────────────────────────────────┐
│                        API Layer (GieudexPol.API)             │
│  ┌─────────────────┐            ┌─────────────────────────────┐  │
│  │  Kontrolery    │            │  Middleware (JWT, CORS)     │  │
│  └─────────────────┘            └─────────────────────────────┘  │
└───────────────────────────────────────────────────────────────┘
                                 │
                                 ▼
┌───────────────────────────────────────────────────────────────┐
│                     Application Layer (GieudexPol.Application)  │
│  ┌─────────────────┐            ┌─────────────────────────────┐  │
│  │  Usługi Biznesowe│            │  Interfejsy Repozytoriów   │  │
│  └─────────────────┘            └─────────────────────────────┘  │
└───────────────────────────────────────────────────────────────┘
                                 │
                                 ▼
┌───────────────────────────────────────────────────────────────┐
│                  Domain Layer (GieudexPol.Domain)             │
│  ┌─────────────────┐            ┌─────────────────────────────┐  │
│  │  Entities       │            │  Reguły Biznesowe           │  │
│  └─────────────────┘            └─────────────────────────────┘  │
└───────────────────────────────────────────────────────────────┘
                                 │
                                 ▼
┌───────────────────────────────────────────────────────────────┐
│                Infrastructure Layer (GieudexPol.Infrastructure)│
│  ┌─────────────────┐            ┌─────────────────────────────┐  │
│  │  Repozytoria    │            │  Entity Framework Core      │  │
│  └─────────────────┘            └─────────────────────────────┘  │
└───────────────────────────────────────────────────────────────┘
```

### 📂 **Struktura katalogów**

```
GieudexPol/
├── GieudexPol.Domain/          # Warstwa Domain (Entities, reguły biznesowe)
├── GieudexPol.Application/    # Warstwa Application (Usługi, Interfejsy)
├── GieudexPol.Infrastructure/  # Warstwa Infrastructure (Repozytoria, EF Core)
├── GieudexPol.API/             # Warstwa API (Kontrolery, Middleware)
└── GieudexPol.Frontend/        # Frontend Angular
```

---

## 🔧 **Co zrobiliśmy**

### 🔄 **Rozwiązane problemy**

1. **Frontend:**
   - Zbudowany w Angularze z użyciem TypeScript
   - Komponenty: dashboard, wallet, trading-form, alerts, history, chart, orderbook
   - Routing zdefiniowany w `app.routes.ts`

2. **Backend:**
   - API REST z kontrolerami dla wszystkich funkcjonalności
   - Autoryzacja JWT
   - Obsługa CORS
   - Swagger UI dla dokumentacji API

3. **Baza danych:**
   - SQL Server z Entity Framework Core
   - Migracje bazy danych
   - Encje: User, Currency, ExchangeRate, Transaction, Wallet, UserAlert

4. **Docker:**
   - Wielostopniowy Dockerfile
   - Kontenery dla frontendu i backendu
   - Poprawne kopiowanie plików statycznych
   - Wdrożenie na Railway.app

### 🎯 **Najważniejsze zmiany**

1. **Dockerfile:**
   - Poprawione kopiowanie plików frontendu do katalogu `wwwroot`
   - Dodana weryfikacja kopiowania plików

2. **Program.cs:**
   - Dodane middleware dla serwowania plików statycznych
   - Zapewniony poprawny routing Angulara z `MapFallbackToFile("index.html")`

3. **Wdrożenie:**
   - Aplikacja dostępna na Railway.app
   - Automatyczne budowanie i wdrażanie z GitHub
   - Publiczny adres URL: `https://gieudexpol-production.up.railway.app`


---

__Kroki do uruchomienia aplikacji GieudexPol lokalnie:__

---

### 📋 __Wymagania wstępne__

1. Zainstalowany __Docker__ (wersja 20.10 lub nowsza)
2. Zainstalowany __Docker Compose__ (wersja 1.29 lub nowsza)
3. Zainstalowany __Git__ (do klonowania repozytorium)

---

### 🛠️ __Kroki uruchomienia:__

#### 1. __Sklonuj repozytorium__

```bash
git clone https://github.com/kacluc/GieudexPol.git
cd GieudexPol
```

#### 2. __Uruchom aplikację__

```bash
docker-compose up -d
```

- Ta komenda:

  - Buduje obrazy Docker
  - Uruchamia kontenery z bazą danych, backendem i frontendem
  - `-d` uruchamia w tle (detached mode)

#### 3. __Sprawdź status kontenerów__

```bash
docker-compose ps
```

- Powinieneś zobaczyć trzy kontenery:

  - `gieudexpol-db` (baza danych SQL Server)
  - `gieudexpol-api` (backend .NET)
  - `gieudexpol-nginx` (frontend Angular)

#### 4. __Dostęp do aplikacji__

Otwórz w przeglądarce:

- __Frontend:__ [](http://localhost)<http://localhost>
- __Backend API:__ [](http://localhost:5010)<http://localhost:5010>
- __Dokumentacja API (Swagger):__ [](http://localhost:5010/swagger/index.html)<http://localhost:5010/swagger/index.html>

---

### 🔄 __Jeśli potrzebujesz zbudować aplikację od nowa__

```bash
# Zatrzymaj i usuń istniejące kontenery
docker-compose down

# Uruchom ponownie
docker-compose up -d --build
```

---

### 🔧 __Jeśli wystąpią problemy:__

#### 1. __Zastosuj migracje bazy danych__

```bash
docker-compose exec gieudexpol-api dotnet ef database update --project ../GieudexPol.Infrastructure --startup-project .
```

#### 2. __Sprawdź logi aplikacji__

```bash
docker-compose logs
```

#### 3. __Zatrzymaj aplikację__

```bash
docker-compose down
```

---

### 💡 __Szczegóły techniczne:__

- Aplikacja używa __Docker Compose__ do uruchamiania wszystkich komponentów w izolowanych kontenerach
- __Frontend__ jest serwowany przez NGINX
- __Backend__ jest napisany w .NET 8 Core
- __Baza danych__ to SQL Server
- __Swagger UI__ dostępny pod `/swagger` dla dokumentacji API

---

### 🎯 __Podsumowanie:__

1. Sklonuj repozytorium
2. Uruchom `docker-compose up -d`
3. Otwórz aplikację w przeglądarce pod `http://localhost`








## 🚀 Instalacja i Uruchomienie (Dev Environment)

1. **Wymagania:** .NET 8 SDK, Node.js, MS SQL Server.
2. **Backend:**
   ```bash
   dotnet restore
   dotnet ef database update
   dotnet run --project GieudexPol.API

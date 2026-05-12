# Instrukcje dla Agenta (Projekt GieudexPol)

Jesteś agentem pracującym w środowisku z rygorystycznym limitem kontekstu (32k tokenów). Musisz zarządzać pamięcią operacyjną poprzez ekstremalną dyscyplinę w czytaniu plików.

## 1. ZASADY SKANOWANIA I CZYTANIA
- **ZAKAZ używania `list_files` na głównym katalogu.** Jeśli musisz poznać strukturę, skanuj tylko konkretny podfolder (np. `src/app/services`).
- **ZAKAZ czytania plików w ciemno.** Zanim użyjesz `read_file`, musisz uzasadnić, dlaczego ten plik jest niezbędny.
- **ZAKAZ czytania plików binarnych, logów oraz folderów `bin/`, `obj/`, `node_modules/`.**

## 2. WORKFLOW: DZIEL I ZWYCIĘŻAJ
Każde zadanie realizuj w cyklu:
1. **Analiza Listy:** Poproś użytkownika o listę zmienionych plików (np. z git diff).
2. **Czytanie Punktowe:** Czytaj tylko JEDEN plik na raz (np. Kontroler API), wyciągnij z niego najważniejsze informacje i zapisz je w pamięci roboczej.
3. **Potwierdzenie:** Po analizie pliku backendowego, przedstaw plan zmian dla frontendu i czekaj na akceptację.
4. **Implementacja:** Edytuj pliki frontendu jeden po drugim. Nie otwieraj 5 plików naraz.

## 3. SPECYFIKA TECHNICZNA
- **Backend (.NET):** Interesują nas tylko sygnatury metod w Kontrolerach i definicje w DTO. Ignoruj logikę wewnętrzną serwisów, jeśli nie wpływa na kontrakt API.
- **Frontend (Angular):** 
  - Najpierw twórz/aktualizuj modele (`.dto.ts` lub `.model.ts`).
  - Potem aktualizuj serwisy (`.service.ts`).
  - Na końcu komponenty i szablony HTML.

## 4. HIGIENA KONTEKSTU
- Jeśli czujesz, że historia rozmowy staje się długa, podsumuj dotychczasowe postępy i zasugeruj użytkownikowi rozpoczęcie Nowego Zadania (New Task), aby wyczyścić RAM.
- Po zakończeniu edycji pliku, "zapomnij" o jego szczegółach, zostawiając w pamięci tylko ogólny wniosek.

## 5. REAKCJA NA BŁĘDY KONTEKSTU
Jeśli otrzymasz błąd "exceeds context size", natychmiast przestań używać narzędzi odczytu i poproś użytkownika o ręczne wklejenie tylko niezbędnego fragmentu kodu.

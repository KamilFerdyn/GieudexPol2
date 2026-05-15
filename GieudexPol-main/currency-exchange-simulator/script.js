document.addEventListener('DOMContentLoaded', function () {
    // Funkcja do pobierania kursów walutowych z NBP API
    async function fetchExchangeRates() {
        try {
            const response = await fetch('https://api.nbp.pl/api/exchangerates/tables/a/?format=json');
            const data = await response.json();
            const rates = {};
            data[0].rates.forEach(rate => {
                rates[rate.code] = rate.mid;
            });
            return rates;
        } catch (error) {
            console.error('Błąd pobierania kursów z NBP:', error);
            // W przypadku błędu użyj domyślnych kursów
            return {};
        }
    }

    // Pobierz kursy walutowe przy załadowaniu strony
    let exchangeRates = {};
    fetchExchangeRates().then(rates => {
        exchangeRates = rates;
        console.log("Pobrane kursy:", rates);
    });

    // Funkcja do konwersji kwoty z waluty źródłowej na PLN
    function convertToPLN(amount, currency) {
        if (!exchangeRates[currency]) {
            alert(`Brak kursu dla ${currency}. Użyto kursu domyślnego.`);
            return amount * 4.5; // Domyślny kurs dla walut nieznanych
        }
        if (currency === 'PLN') {
            return amount;
        }
        return amount * exchangeRates[currency];
    }

    // Funkcja do konwersji kwoty z PLN na walutę docelową
    function convertFromPLN(amountInPLN, currency) {
        if (!exchangeRates[currency]) {
            alert(`Brak kursu dla ${currency}. Użyto kursu domyślnego.`);
            return amountInPLN / 4.5; // Domyślny kurs dla walut nieznanych
        }
        if (currency === 'PLN') {
            return amountInPLN;
        }
        return amountInPLN / exchangeRates[currency];
    }

    // Funkcja do obliczania wymiany walut
    function calculateExchange() {
        const amountInput = document.getElementById('amount');
        const sourceCurrency = document.getElementById('sourceCurrency');
        const targetCurrency = document.getElementById('targetCurrency');
        const feeInput = document.getElementById('fee');
        const resultAmountElement = document.getElementById('resultAmount');
        const resultFeeElement = document.getElementById('resultFee');
        const resultTotalElement = document.getElementById('resultTotal');

        const amount = parseFloat(amountInput.value);
        const fee = parseFloat(feeInput.value);
        const source = sourceCurrency.value;
        const target = targetCurrency.value;

        if (isNaN(amount) || isNaN(fee) || !source || !target) {
            alert('Proszę uzupełnić wszystkie pola poprawnie.');
            return;
        }

        // Konwersja kwoty z waluty źródłowej na PLN
        const amountInPLN = convertToPLN(amount, source);

        // Konwersja kwoty z PLN na walutę docelową
        const exchangedAmount = convertFromPLN(amountInPLN, target);

        // Obliczanie prowizji
        const feeAmount = exchangedAmount * (fee / 100);

        // Obliczanie kwoty netto
        const totalAmount = exchangedAmount - feeAmount;

        // Wyświetlanie wyników
        resultAmountElement.textContent = `${exchangedAmount.toFixed(2)} ${target}`;
        resultFeeElement.textContent = `Prowizja: ${feeAmount.toFixed(2)} ${target} (${fee}%)`;
        resultTotalElement.textContent = `Kwota netto: ${totalAmount.toFixed(2)} ${target}`;
    }

    // Dodanie event listenera do przycisku "Oblicz"
    document.getElementById('calculate').addEventListener('click', calculateExchange);
});
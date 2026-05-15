document.addEventListener('DOMContentLoaded', function() {
    // Kursy walut względem PLN (1 PLN = X waluty)
    const exchangeRates = {
        PLN: 1,    // Referencyjna waluta
        USD: 4.25, // 1 USD = 4.25 PLN
        EUR: 4.50, // 1 EUR = 4.50 PLN
        GBP: 5.30, // 1 GBP = 5.30 PLN
        JPY: 0.032, // 1 JPY = 0.032 PLN (1 PLN = ~31.25 JPY)
        CHF: 4.60  // 1 CHF = 4.60 PLN
    };

    // Funkcja do konwersji kwoty z waluty źródłowej na PLN
    function convertToPLN(amount, currency) {
        if (currency === 'PLN') {
            return amount;
        }
        return amount * exchangeRates[currency];
    }

    // Funkcja do konwersji kwoty z PLN na walutę docelową
    function convertFromPLN(amountInPLN, currency) {
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
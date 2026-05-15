document.addEventListener('DOMContentLoaded', function() {
    // Kursy walut (PLN do innych walut)
    const exchangeRates = {
        USD: 4.25,  // 1 USD = 4.25 PLN
        EUR: 4.50,  // 1 EUR = 4.50 PLN
        GBP: 5.30,  // 1 GBP = 5.30 PLN
        JPY: 0.032, // 1 JPY = 0.032 PLN
        CHF: 4.60   // 1 CHF = 4.60 PLN
    };

    // Funkcja do obliczania wymiany walut
    function calculateExchange() {
        const amountInput = document.getElementById('amount');
        const targetCurrency = document.getElementById('targetCurrency');
        const feeInput = document.getElementById('fee');
        const resultAmountElement = document.getElementById('resultAmount');
        const resultFeeElement = document.getElementById('resultFee');
        const resultTotalElement = document.getElementById('resultTotal');

        const amount = parseFloat(amountInput.value);
        const fee = parseFloat(feeInput.value);
        const currency = targetCurrency.value;

        if (isNaN(amount) || isNaN(fee) || !currency) {
            alert('Proszę uzupełnić wszystkie pola poprawnie.');
            return;
        }

        // Obliczanie kwoty w walucie docelowej
        let exchangedAmount;
        if (currency === 'JPY') {
            // Dla JPY kurs jest odwrócony (1 PLN = 31.25 JPY)
            exchangedAmount = amount / exchangeRates[currency];
        } else {
            exchangedAmount = amount / exchangeRates[currency];
        }

        // Obliczanie prowizji
        const feeAmount = exchangedAmount * (fee / 100);

        // Obliczanie kwoty netto
        const totalAmount = exchangedAmount - feeAmount;

        // Wyświetlanie wyników
        resultAmountElement.textContent = `${exchangedAmount.toFixed(2)} ${currency}`;
        resultFeeElement.textContent = `Prowizja: ${feeAmount.toFixed(2)} ${currency} (${fee}%)`;
        resultTotalElement.textContent = `Kwota netto: ${totalAmount.toFixed(2)} ${currency}`;
    }

    // Dodanie event listenera do przycisku "Oblicz"
    document.getElementById('calculate').addEventListener('click', calculateExchange);
});
namespace WorldRank.Domain;

public class CurrencyRates
{
    public string Currency { get; private set; }
    public decimal Rate { get; private set; }
    public DateTime Date { get; private set; }

    private CurrencyRates()
    {
        Currency = string.Empty;
    }

    public CurrencyRates(string currency, decimal rate, DateTime date)
    {
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be empty.", nameof(currency));

        Currency = currency;
        Rate = rate;
        Date = date;
    }
}

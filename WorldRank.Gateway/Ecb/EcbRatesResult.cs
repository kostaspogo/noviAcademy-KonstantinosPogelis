namespace WorldRank.Gateway.Ecb
{
    public record EcbRate(string Currency, decimal Rate);

    public record EcbRatesResult(DateTime Date, IReadOnlyList<EcbRate> Rates);
}

namespace WorldRank.Gateway.Ecb
{
    public interface IEcbRatesClient
    {
        Task<EcbRatesResult> GetLatestRatesAsync(CancellationToken ct = default);
    }
}

using Quartz;
using WorldRank.Domain;
using WorldRank.Gateway.Ecb;
using WorldRank.Infrastructure.Persistence.Context;

namespace WorldRank.API.Jobs
{
    [DisallowConcurrentExecution]
    public class EcbRatesSyncJob : IJob
    {
        private readonly IEcbRatesClient _ecb;
        private readonly WorldRankDbContext _db;
        private readonly ILogger<EcbRatesSyncJob> _logger;

        public EcbRatesSyncJob(IEcbRatesClient ecb, WorldRankDbContext db, ILogger<EcbRatesSyncJob> logger)
        {
            _ecb = ecb;
            _db = db;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var ct = context.CancellationToken;
            _logger.LogInformation("ECB rates sync started");

            var result = await _ecb.GetLatestRatesAsync(ct);

            var added = 0;
            foreach (var rate in result.Rates)
            {
                var exists = await _db.CurrencyRates.FindAsync(new object[] { rate.Currency, result.Date }, ct);
                if (exists is null)
                {
                    _db.CurrencyRates.Add(new CurrencyRates(rate.Currency, rate.Rate, result.Date));
                    added++;
                }
            }

            await _db.SaveChangesAsync(ct);
            _logger.LogInformation("ECB rates sync finished: {Added} new rates for {Date:yyyy-MM-dd}", added, result.Date);
        }
    }
}

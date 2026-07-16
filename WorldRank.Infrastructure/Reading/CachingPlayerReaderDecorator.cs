using Microsoft.Extensions.Logging;
using WorldRank.Application.Interfaces;
using WorldRank.Domain;

namespace WorldRank.Infrastructure.Reading
{
    public class CachingPlayerReaderDecorator : IPlayerReader
    {
        private static readonly TimeSpan Ttl = TimeSpan.FromSeconds(60);
        private static string PlayerKey(int id) => $"player:{id}";

        private readonly IPlayerReader _inner;
        private readonly ICache _cache;
        private readonly ILogger<CachingPlayerReaderDecorator> _logger;

        public CachingPlayerReaderDecorator(
            IPlayerReader inner,
            ICache cache,
            ILogger<CachingPlayerReaderDecorator> logger)
        {
            _inner = inner;
            _cache = cache;
            _logger = logger;
        }

        public async Task<Player?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            if (_cache.TryGet(PlayerKey(id), out Player? cached) && cached is not null)
            {
                _logger.LogInformation("Cache HIT  player {PlayerId}", id);
                return cached;
            }

            _logger.LogInformation("Cache MISS player {PlayerId}", id);
            var player = await _inner.GetByIdAsync(id, ct);
            if (player is not null)
            {
                _cache.Set(PlayerKey(id), player, Ttl);
            }

            return player;
        }
    }
}

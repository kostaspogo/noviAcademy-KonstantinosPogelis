using Microsoft.Extensions.Logging;
using WorldRank.Application.Interfaces;
using WorldRank.Domain;

namespace WorldRank.Application.Services
{
    public class PlayerService : IPlayerService
    {
        private static readonly TimeSpan Ttl = TimeSpan.FromSeconds(60);
        private const string AllPlayersKey = "players:all";
        private static string PlayerKey(int id) => $"player:{id}";

        private readonly IPlayerRepository _players;
        private readonly ICache _cache;
        private readonly ILogger<PlayerService> _logger;

        public PlayerService(IPlayerRepository players, ICache cache, ILogger<PlayerService> logger)
        {
            _players = players;
            _cache = cache;
            _logger = logger;
        }

        public async Task<Player> CreateAsync(string name, int score, CancellationToken ct = default)
        {
            var all = await _players.GetAllPlayersAsync(ct);
            var nextId = all.Count == 0 ? 1 : all.Max(p => p.Id) + 1;

            var player = new Player(nextId, name);
            player.AddScore(score);

            await _players.AddPlayerAsync(player, ct);
            _cache.Set(PlayerKey(player.Id), player, Ttl);
            _cache.Remove(AllPlayersKey);
            _logger.LogInformation("Player {PlayerId} created; cache write-through, list invalidated", player.Id);
            return player;
        }

        public async Task<Player?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            if (_cache.TryGet(PlayerKey(id), out Player? cached) && cached is not null)
            {
                _logger.LogInformation("Cache HIT  player {PlayerId}", id);
                return cached;
            }

            _logger.LogInformation("Cache MISS player {PlayerId} - loading from repository", id);
            var player = await _players.FindPlayerAsync(id, ct);
            if (player is not null)
                _cache.Set(PlayerKey(id), player, Ttl);
            return player;
        }

        public async Task<IReadOnlyList<Player>> GetAllAsync(CancellationToken ct = default)
        {
            if (_cache.TryGet(AllPlayersKey, out IReadOnlyList<Player>? cached) && cached is not null)
            {
                _logger.LogInformation("Cache HIT  all players");
                return cached;
            }

            _logger.LogInformation("Cache MISS all players - loading from repository");
            var players = await _players.GetAllPlayersAsync(ct);
            _cache.Set(AllPlayersKey, players, Ttl);
            return players;
        }
    }
}

using Microsoft.Extensions.Caching.Memory;
using NLog;
using WorldRank.Application.Interfaces;
using WorldRank.Domain;

namespace WorldRank.Infrastructure.Repositories
{
	public class InMemoryPlayerRepository : IPlayerRepository
	{
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		private readonly IMemoryCache _cache;

		private List<Player> _players;

		public InMemoryPlayerRepository(IMemoryCache cache)
		{
			_players = new List<Player>();
			_cache = cache;
		}

		public void AddPlayer(Player player)
		{
			_players.Add(player);
			_logger.Info("Player {PlayerId} ({Name}) added with score {Score}", player.Id, player.Name, player.Score);
		}


        public IEnumerable<Player> GetAllPlayers()
        {
            if (_cache.TryGetValue("AllPlayersKey", out IReadOnlyList<Player>? cached) && cached is not null)
            {
                return cached;
            }

            var players = _players.ToList();

            _cache.Set("AllPlayersKey", players, TimeSpan.FromSeconds(60));

            return players;
        }

        public void DeletePlayer(int playerId)
		{
			var player = _players.Where(item => item.Id == playerId).FirstOrDefault();

			if (player is null)
			{
				_logger.Warn("Delete skipped: player {PlayerId} not found", playerId);
				return;
			}

			_players.Remove(player);
			_logger.Info("Player {PlayerId} deleted", playerId);
		}

		public Player? FindPlayer(int playerId)
		{
			return _players.Where(item => item.Id == playerId).FirstOrDefault();
		}

		public IEnumerable<IGrouping<int, Player>> GroupPlayersByScore()
		{
			return _players
				.GroupBy(player => player.Score)
				.OrderByDescending(group => group.Key);
		}
	}
}

using NLog;
using WorldRank.Application.Interfaces;
using WorldRank.Domain;

namespace WorldRank.Infrastructure.Repositories
{
	public class InMemoryPlayerRepository : IPlayerRepository
	{
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		private readonly List<Player> _players = new List<Player>();

		public Task AddPlayerAsync(Player player, CancellationToken ct = default)
		{
			_players.Add(player);
			_logger.Info("Player {PlayerId} ({Name}) added with score {Score}", player.Id, player.Name, player.Score);
			return Task.CompletedTask;
		}

		public Task<IReadOnlyList<Player>> GetAllPlayersAsync(CancellationToken ct = default)
		{
			return Task.FromResult<IReadOnlyList<Player>>(_players.ToList());
		}

		public Task DeletePlayerAsync(int playerId, CancellationToken ct = default)
		{
			var player = _players.FirstOrDefault(item => item.Id == playerId);

			if (player is null)
			{
				_logger.Warn("Delete skipped: player {PlayerId} not found", playerId);
				return Task.CompletedTask;
			}

			_players.Remove(player);
			_logger.Info("Player {PlayerId} deleted", playerId);
			return Task.CompletedTask;
		}

		public Task<Player?> FindPlayerAsync(int playerId, CancellationToken ct = default)
		{
			return Task.FromResult(_players.FirstOrDefault(item => item.Id == playerId));
		}

		public Task<IEnumerable<IGrouping<int, Player>>> GroupPlayersByScoreAsync(CancellationToken ct = default)
		{
			IEnumerable<IGrouping<int, Player>> grouped = _players
				.GroupBy(player => player.Score)
				.OrderByDescending(group => group.Key);

			return Task.FromResult(grouped);
		}
	}
}

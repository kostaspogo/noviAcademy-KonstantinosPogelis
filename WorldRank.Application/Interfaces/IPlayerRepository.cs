using WorldRank.Domain;

namespace WorldRank.Application.Interfaces
{
	public interface IPlayerRepository
	{
		Task AddPlayerAsync(Player player, CancellationToken ct = default);

		Task<IReadOnlyList<Player>> GetAllPlayersAsync(CancellationToken ct = default);

		Task DeletePlayerAsync(int playerId, CancellationToken ct = default);

		Task<Player?> FindPlayerAsync(int playerId, CancellationToken ct = default);

		Task<IEnumerable<IGrouping<int, Player>>> GroupPlayersByScoreAsync(CancellationToken ct = default);
	}
}

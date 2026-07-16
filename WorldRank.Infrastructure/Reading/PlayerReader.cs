using WorldRank.Application.Interfaces;
using WorldRank.Domain;

namespace WorldRank.Infrastructure.Reading
{
    public class PlayerReader : IPlayerReader
    {
        private readonly IPlayerRepository _players;

        public PlayerReader(IPlayerRepository players)
        {
            _players = players;
        }

        public Task<Player?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return _players.FindPlayerAsync(id, ct);
        }
    }
}

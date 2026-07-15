using WorldRank.Domain;

namespace WorldRank.Application.Interfaces
{
    public interface IPlayerService
    {
        Task<Player> CreateAsync(string name, int score, CancellationToken ct = default);

        Task<Player?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<IReadOnlyList<Player>> GetAllAsync(CancellationToken ct = default);
    }
}

using WorldRank.Domain;

namespace WorldRank.Application.Interfaces
{
    public interface IPlayerReader
    {
        Task<Player?> GetByIdAsync(int id, CancellationToken ct = default);
    }
}

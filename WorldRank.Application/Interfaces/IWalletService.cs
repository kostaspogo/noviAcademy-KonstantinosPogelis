using WorldRank.Domain;
using WorldRank.Domain.Enums;

namespace WorldRank.Application.Interfaces
{
    public interface IWalletService
    {
        Task<Wallet> CreateAsync(int playerId, Currency currency, decimal balance, CancellationToken ct = default);

        Task<Wallet?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<Wallet?> DepositAsync(int id, decimal amount, CancellationToken ct = default);
    }
}

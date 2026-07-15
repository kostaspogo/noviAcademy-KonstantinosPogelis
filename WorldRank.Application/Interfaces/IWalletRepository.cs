using WorldRank.Domain;
using WorldRank.Domain.Enums;

namespace WorldRank.Application.Interfaces
{
	public interface IWalletRepository
	{
		Task AddAsync(Wallet wallet, CancellationToken ct = default);

		Task<Wallet?> GetByIdAsync(int id, CancellationToken ct = default);

		Task<IReadOnlyList<Wallet>> GetAllWalletsByPlayerIdAsync(int playerId, CancellationToken ct = default);

		Task<Wallet> GetWalletAsync(int playerId, Currency currency, CancellationToken ct = default);

		Task UpdateBalanceAsync(int playerId, Currency currency, decimal newBalance, CancellationToken ct = default);

		Task DepositAsync(int playerId, Currency currency, decimal amount, CancellationToken ct = default);

		Task WithdrawAsync(int playerId, Currency currency, decimal amount, CancellationToken ct = default);

		Task BlockAsync(int playerId, Currency currency, CancellationToken ct = default);

		Task UnblockAsync(int playerId, Currency currency, CancellationToken ct = default);
	}
}

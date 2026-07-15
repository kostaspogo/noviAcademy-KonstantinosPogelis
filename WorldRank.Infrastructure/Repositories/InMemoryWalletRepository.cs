using NLog;
using WorldRank.Application.Interfaces;
using WorldRank.Domain;
using WorldRank.Domain.Enums;
using WorldRank.Domain.Exceptions;

namespace WorldRank.Infrastructure.Repositories
{
	public class InMemoryWalletRepository : IWalletRepository
	{
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		private readonly List<Wallet> _wallets = new List<Wallet>();
		private int _nextId = 1;

		public Task AddAsync(Wallet wallet, CancellationToken ct = default)
		{
			var exists = _wallets.Any(item => item.PlayerId == wallet.PlayerId && item.Currency == wallet.Currency);

			if (exists)
			{
				throw new DuplicateWalletException(wallet.PlayerId, wallet.Currency);
			}

			if (wallet.Id == 0)
			{
				wallet.Id = _nextId++;
			}

			_wallets.Add(wallet);
			_logger.Info("Wallet created for player {PlayerId} in {Currency} with balance {Balance}", wallet.PlayerId, wallet.Currency, wallet.Balance);
			return Task.CompletedTask;
		}

		public Task<Wallet?> GetByIdAsync(int id, CancellationToken ct = default)
		{
			return Task.FromResult(_wallets.FirstOrDefault(item => item.Id == id));
		}

		public Task<IReadOnlyList<Wallet>> GetAllWalletsByPlayerIdAsync(int playerId, CancellationToken ct = default)
		{
			return Task.FromResult<IReadOnlyList<Wallet>>(_wallets.Where(item => item.PlayerId == playerId).ToList());
		}

		public Task<Wallet> GetWalletAsync(int playerId, Currency currency, CancellationToken ct = default)
		{
			var wallet = _wallets.SingleOrDefault(item => item.PlayerId == playerId && item.Currency == currency);

			if (wallet is null)
			{
				throw new WalletNotFoundException(playerId, currency);
			}

			return Task.FromResult(wallet);
		}

		public async Task UpdateBalanceAsync(int playerId, Currency currency, decimal newBalance, CancellationToken ct = default)
		{
			var wallet = await GetWalletAsync(playerId, currency, ct);
			wallet.SetBalance(newBalance);
			_logger.Info("Player {PlayerId} {Currency} wallet balance set to {Balance}", playerId, currency, newBalance);
		}

		public async Task DepositAsync(int playerId, Currency currency, decimal amount, CancellationToken ct = default)
		{
			var wallet = await GetWalletAsync(playerId, currency, ct);
			wallet.Deposit(amount);
			_logger.Info("Deposited {Amount} to player {PlayerId} {Currency} wallet (balance {Balance})", amount, playerId, currency, wallet.Balance);
		}

		public async Task WithdrawAsync(int playerId, Currency currency, decimal amount, CancellationToken ct = default)
		{
			var wallet = await GetWalletAsync(playerId, currency, ct);
			wallet.Withdraw(amount);
			_logger.Info("Withdrew {Amount} from player {PlayerId} {Currency} wallet (balance {Balance})", amount, playerId, currency, wallet.Balance);
		}

		public async Task BlockAsync(int playerId, Currency currency, CancellationToken ct = default)
		{
			var wallet = await GetWalletAsync(playerId, currency, ct);
			wallet.Block();
			_logger.Info("Player {PlayerId} {Currency} wallet blocked", playerId, currency);
		}

		public async Task UnblockAsync(int playerId, Currency currency, CancellationToken ct = default)
		{
			var wallet = await GetWalletAsync(playerId, currency, ct);
			wallet.Unblock();
			_logger.Info("Player {PlayerId} {Currency} wallet unblocked", playerId, currency);
		}
	}
}

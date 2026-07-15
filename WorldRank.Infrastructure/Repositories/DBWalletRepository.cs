using Microsoft.EntityFrameworkCore;
using NLog;
using WorldRank.Application.Interfaces;
using WorldRank.Domain;
using WorldRank.Domain.Enums;
using WorldRank.Domain.Exceptions;
using WorldRank.Infrastructure.Persistence.Context;

namespace WorldRank.Infrastructure.Repositories
{
    // Database-backed υλοποίηση του IWalletRepository μέσω Entity Framework Core.
    // Ίδια συμπεριφορά/queries με το InMemoryWalletRepository, αλλά μόνιμη αποθήκευση στη βάση.
    public class DBWalletRepository : IWalletRepository
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly WorldRankDbContext _context;

        public DBWalletRepository(WorldRankDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Wallet wallet, CancellationToken ct = default)
        {
            var exists = await _context.Wallets.AnyAsync(item => item.PlayerId == wallet.PlayerId && item.Currency == wallet.Currency, ct);

            if (exists)
            {
                throw new DuplicateWalletException(wallet.PlayerId, wallet.Currency);
            }

            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync(ct);
            _logger.Info("Wallet created for player {PlayerId} in {Currency} with balance {Balance}", wallet.PlayerId, wallet.Currency, wallet.Balance);
        }

        public async Task<Wallet?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _context.Wallets.FirstOrDefaultAsync(item => item.Id == id, ct);
        }

        public async Task<IReadOnlyList<Wallet>> GetAllWalletsByPlayerIdAsync(int playerId, CancellationToken ct = default)
        {
            return await _context.Wallets.Where(item => item.PlayerId == playerId).ToListAsync(ct);
        }

        public async Task<Wallet> GetWalletAsync(int playerId, Currency currency, CancellationToken ct = default)
        {
            var wallet = await _context.Wallets.SingleOrDefaultAsync(item => item.PlayerId == playerId && item.Currency == currency, ct);

            if (wallet is null)
            {
                throw new WalletNotFoundException(playerId, currency);
            }

            return wallet;
        }

        public async Task UpdateBalanceAsync(int playerId, Currency currency, decimal newBalance, CancellationToken ct = default)
        {
            var wallet = await GetWalletAsync(playerId, currency, ct);
            wallet.SetBalance(newBalance);
            await _context.SaveChangesAsync(ct);
            _logger.Info("Player {PlayerId} {Currency} wallet balance set to {Balance}", playerId, currency, newBalance);
        }

        public async Task DepositAsync(int playerId, Currency currency, decimal amount, CancellationToken ct = default)
        {
            var wallet = await GetWalletAsync(playerId, currency, ct);
            wallet.Deposit(amount);
            await _context.SaveChangesAsync(ct);
            _logger.Info("Deposited {Amount} to player {PlayerId} {Currency} wallet (balance {Balance})", amount, playerId, currency, wallet.Balance);
        }

        public async Task WithdrawAsync(int playerId, Currency currency, decimal amount, CancellationToken ct = default)
        {
            var wallet = await GetWalletAsync(playerId, currency, ct);
            wallet.Withdraw(amount);
            await _context.SaveChangesAsync(ct);
            _logger.Info("Withdrew {Amount} from player {PlayerId} {Currency} wallet (balance {Balance})", amount, playerId, currency, wallet.Balance);
        }

        public async Task BlockAsync(int playerId, Currency currency, CancellationToken ct = default)
        {
            var wallet = await GetWalletAsync(playerId, currency, ct);
            wallet.Block();
            await _context.SaveChangesAsync(ct);
            _logger.Info("Player {PlayerId} {Currency} wallet blocked", playerId, currency);
        }

        public async Task UnblockAsync(int playerId, Currency currency, CancellationToken ct = default)
        {
            var wallet = await GetWalletAsync(playerId, currency, ct);
            wallet.Unblock();
            await _context.SaveChangesAsync(ct);
            _logger.Info("Player {PlayerId} {Currency} wallet unblocked", playerId, currency);
        }
    }
}

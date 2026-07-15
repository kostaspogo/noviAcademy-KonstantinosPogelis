using Microsoft.Extensions.Logging;
using WorldRank.Application.Interfaces;
using WorldRank.Domain;
using WorldRank.Domain.Enums;

namespace WorldRank.Application.Services
{
    public class WalletService : IWalletService
    {
        private static readonly TimeSpan Ttl = TimeSpan.FromSeconds(60);
        private static string WalletKey(int id) => $"wallet:{id}";

        private readonly IWalletRepository _wallets;
        private readonly ICache _cache;
        private readonly ILogger<WalletService> _logger;

        public WalletService(IWalletRepository wallets, ICache cache, ILogger<WalletService> logger)
        {
            _wallets = wallets;
            _cache = cache;
            _logger = logger;
        }

        public async Task<Wallet> CreateAsync(int playerId, Currency currency, decimal balance, CancellationToken ct = default)
        {
            var wallet = new Wallet(playerId, currency, balance);
            await _wallets.AddAsync(wallet, ct);
            _cache.Set(WalletKey(wallet.Id), wallet, Ttl);
            _logger.LogInformation("Wallet {WalletId} created (player {PlayerId} {Currency}); cache write-through", wallet.Id, playerId, currency);
            return wallet;
        }

        public async Task<Wallet?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            if (_cache.TryGet(WalletKey(id), out Wallet? cached) && cached is not null)
            {
                _logger.LogInformation("Cache HIT  wallet {WalletId}", id);
                return cached;
            }

            _logger.LogInformation("Cache MISS wallet {WalletId} - loading from repository", id);
            var wallet = await _wallets.GetByIdAsync(id, ct);
            if (wallet is not null)
                _cache.Set(WalletKey(id), wallet, Ttl);
            return wallet;
        }

        public async Task<Wallet?> DepositAsync(int id, decimal amount, CancellationToken ct = default)
        {
            var wallet = await _wallets.GetByIdAsync(id, ct);
            if (wallet is null)
                return null;

            await _wallets.DepositAsync(wallet.PlayerId, wallet.Currency, amount, ct);

            var updated = await _wallets.GetByIdAsync(id, ct);
            _cache.Set(WalletKey(id), updated!, Ttl);
            _logger.LogInformation("Deposited {Amount} to wallet {WalletId}; cache updated", amount, id);
            return updated;
        }
    }
}

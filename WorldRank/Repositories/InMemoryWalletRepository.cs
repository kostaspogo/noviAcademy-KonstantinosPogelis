namespace WorldRank;

public class InMemoryWalletRepository : IWalletRepository
{
    private readonly Dictionary<int, Dictionary<Currency, Wallet>> _walletsByPlayerId = new();

    public void AddWallet(Wallet wallet, int playerId)
    {
        if (!_walletsByPlayerId.TryGetValue(playerId, out var wallets))
        {
            wallets = new Dictionary<Currency, Wallet>();
            _walletsByPlayerId[playerId] = wallets;
        }

        if (wallets.ContainsKey(wallet.Currency))
        {
            throw new InvalidOperationException($"Wallet with currency {wallet.Currency} already exists for player {playerId}");
        }

        wallets.Add(wallet.Currency, wallet);
    }

    public IEnumerable<Wallet> GetByPlayer(int playerId) =>
        _walletsByPlayerId.TryGetValue(playerId, out var wallets) ? wallets.Values : Enumerable.Empty<Wallet>();
}

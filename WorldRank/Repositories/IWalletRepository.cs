namespace WorldRank;

public interface IWalletRepository
{
    void AddWallet(Wallet wallet, int playerId);
    IEnumerable<Wallet> GetByPlayer(int playerId);
}

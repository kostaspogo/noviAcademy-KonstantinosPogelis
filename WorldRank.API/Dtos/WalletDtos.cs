using WorldRank.Domain;
using WorldRank.Domain.Enums;

namespace WorldRank.API.Dtos
{
    public record CreateWalletRequest(int PlayerId, Currency Currency, decimal Balance);

    public record DepositRequest(decimal Amount);

    public record WalletResponse(int Id, int PlayerId, Currency Currency, decimal Balance, bool IsBlocked)
    {
        public static WalletResponse From(Wallet wallet) =>
            new(wallet.Id, wallet.PlayerId, wallet.Currency, wallet.Balance, wallet.IsBlocked);
    }
}

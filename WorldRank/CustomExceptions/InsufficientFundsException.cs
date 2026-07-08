namespace WorldRank;

public class InsufficientFundsException : WalletException
{
    public decimal RequestedAmount { get; }
    public decimal AvailableBalance { get; }    

    public InsufficientFundsException(decimal requestedAmount, decimal availableBalance)
        : base($"Insufficient funds: Requested {requestedAmount}, Available {availableBalance}")
    {
        RequestedAmount = requestedAmount;
        AvailableBalance = availableBalance;
    }

}
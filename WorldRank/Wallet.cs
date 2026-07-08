namespace WorldRank;

public class Wallet
{

   public decimal Balance { get; private set; }
   public Currency Currency { get; }
   public bool isBlocked { get; private set; }

   public Wallet (Currency currency)
   {
       Balance = 0;
       Currency = currency;
       isBlocked = false;
   }

   public void Deposit(decimal amount)
   {
       if (isBlocked)
           throw new InvalidOperationException("Wallet is blocked.");

       if (amount <= 0)
           throw new ArgumentOutOfRangeException(nameof(amount), "Deposit amount must be positive.");

       Balance += amount;
   }
   public void Withdraw(decimal amount)
   {
       if (isBlocked)
           throw new InvalidOperationException("Wallet is blocked.");

       if (Balance - amount < 0 )
            throw new InvalidOperationException("Insufficient funds.");

       Balance -= amount;
   }

    public override string ToString()
    {
        return $"{Currency}: {Balance:C} {(isBlocked ? "[BLOCKED]" : "")}";
    }
}

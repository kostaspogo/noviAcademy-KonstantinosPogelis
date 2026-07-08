using WorldRank;

var playerRepo = new InMemoryPlayerRepository();
var walletRepo = new InMemoryWalletRepository();

while (true)
{
	Console.WriteLine("\n=== WorldRank Player Registry ===");
	Console.WriteLine("1. Add player");
	Console.WriteLine("2. List all players");
	Console.WriteLine("3. Find player by name");
	Console.WriteLine("4. Delete player by id");
	Console.WriteLine("5. Add wallet to player");
	Console.WriteLine("6. Deposit to wallet of player");
	Console.WriteLine("7. Withdraw from wallet of player");
	Console.WriteLine("8. List wallets of a player");
	Console.WriteLine("0. Exit");
	Console.Write("> ");

	Action? action = Console.ReadLine() switch
	{
		"1" => AddPlayer,
		"2" => ListPlayers,
		"3" => FindPlayer,
		"4" => DeletePlayer,
		"5" => AddWallet,
		"6" => DepositWallet,
		"7" => WithdrawWallet,
		"8" => ListWallet,
		"0" => null,
		_ => () => Console.WriteLine("Unknown option.")
	};

	if (action is null)
		return; 
	action();
}

void AddPlayer()
{
	Console.Write("Name: ");
	var name = Console.ReadLine();
	if (string.IsNullOrWhiteSpace(name))
	{
		Console.WriteLine("Name cannot be empty.");
		return;
	}

	Console.Write("Score: ");
	var scoreInput = Console.ReadLine();
	if (!int.TryParse(scoreInput, out var score))
	{
		Console.WriteLine("Score must be a whole number.");
		return;
	}


	var player = new Player(name);
    player.UpdateScore(score);

    playerRepo.AddPlayer(player);
    Console.WriteLine($"Player added with Id: {player.Id}");
}

void ListPlayers()
{
	var all = playerRepo.GroupPlayersByScore().SelectMany(g =>g);
	if (!all.Any())
	{
		Console.WriteLine("No players found.");
		return;
	}
	foreach (var p in all)
		Console.WriteLine(p);
}

void FindPlayer()
{
	Console.Write("Player Id: ");
    if (!int.TryParse(Console.ReadLine(), out var id))
    {
        Console.WriteLine("Invalid id.");
        return;
    }

    var player = playerRepo.FindPlayer(id);
    Console.WriteLine(player is null ? "No player found." : player.ToString());
}

void DeletePlayer()
{
	Console.Write("Player Id: ");
	if (!int.TryParse(Console.ReadLine(), out var id))
	{
		Console.WriteLine("Invalid id.");
		return;
	}

	var success = playerRepo.DeletePlayer(id);
	Console.WriteLine(success ? "Player deleted successfully." : "No player found to delete.");
}

void AddWallet()
{
	Console.Write ("Player ID: ");
	if (!int.TryParse(Console.ReadLine(), out var playerId))
	{
		Console.WriteLine("Invalid player ID.");
		return;
	}
	Console.Write("Currency (USD/EUR): ");
	if (!Enum.TryParse<Currency>(Console.ReadLine(), true, out var currency)
		|| !Enum.IsDefined(typeof(Currency), currency))
	{
		Console.WriteLine("Invalid currency.");
		return;
	}

	try
	{
		walletRepo.AddWallet(new Wallet (currency), playerId);
	}
	catch (Exception ex)
	{
		Console.WriteLine($"Error adding wallet: {ex.Message}");
	}
}

void DepositWallet()
{
	Console.Write("Player ID: ");
	if (!int.TryParse(Console.ReadLine(), out var playerId))
	{
		Console.WriteLine("Invalid player ID.");
		return;
	}
	Console.Write("Currency (USD/EUR): ");
	if (!Enum.TryParse<Currency>(Console.ReadLine(), true, out var currency)
		|| !Enum.IsDefined(typeof(Currency), currency))
	{
		Console.WriteLine("Invalid currency.");
		return;
	}
	Console.Write("Amount to deposit: ");
	if (!decimal.TryParse(Console.ReadLine(), out var amount) || amount <= 0)
	{
		Console.WriteLine("Invalid amount.");
		return;
	}

	var wallets = walletRepo.GetByPlayer(playerId);
	var wallet = wallets.FirstOrDefault(w => w.Currency == currency);
	if (wallet is null)
	{
		Console.WriteLine($"No wallet found for currency {currency}.");
		return;
	}

	wallet.Deposit(amount);
	Console.WriteLine($"Deposited {amount} {currency} to player {playerId}'s wallet.");
}

void WithdrawWallet()
{
	Console.Write("Player ID: ");
	if (!int.TryParse(Console.ReadLine(), out var playerId))
	{
		Console.WriteLine("Invalid player ID.");
		return;
	}
	Console.Write("Currency (USD/EUR): ");
	if (!Enum.TryParse<Currency>(Console.ReadLine(), true, out var currency)
		|| !Enum.IsDefined(typeof(Currency), currency))
	{
		Console.WriteLine("Invalid currency.");
		return;
	}
	Console.Write("Amount to withdraw: ");
	if (!decimal.TryParse(Console.ReadLine(), out var amount) || amount <= 0)
	{
		Console.WriteLine("Invalid amount.");
		return;
	}

	var wallets = walletRepo.GetByPlayer(playerId);
	var wallet = wallets.FirstOrDefault(w => w.Currency == currency);
	if (wallet is null)
	{
		Console.WriteLine($"No wallet found for currency {currency}.");
		return;
	}

	try
	{
		wallet.Withdraw(amount);
		Console.WriteLine($"Withdrew {amount} {currency} from player {playerId}'s wallet.");
	}
	catch (Exception ex)
	{
		Console.WriteLine($"Error withdrawing from wallet: {ex.Message}");
	}
}
void ListWallet()
{
	Console.Write("Player ID: ");
	if (!int.TryParse(Console.ReadLine(), out var playerId))
	{
		Console.WriteLine("Invalid player ID.");
		return;
	}

	var wallets = walletRepo.GetByPlayer(playerId);
	if (!wallets.Any())
	{
		Console.WriteLine("No wallets found for this player.");
		return;
	}

	foreach (var wallet in wallets)
		Console.WriteLine(wallet);
}

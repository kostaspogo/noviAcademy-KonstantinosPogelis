namespace WorldRank;

public class Player : IPlayer

{
	private static int _nextId = 1;
	public int Id { get; }
	public string Name { get; }
	public int Score { get; private set; }

	public Player(string name)
	{
		if (string.IsNullOrEmpty(name))
			throw new ArgumentException("Name cannot be null or empty.", nameof(name));

		Id = _nextId++;
		Name = name;
	}

	public void UpdateScore(int newScore)
	{
		if (newScore < 0)
			throw new ArgumentOutOfRangeException(nameof(newScore), "Score cannot be negative.");

		Score = newScore;
	}

	public override string ToString() =>
			$"[{Id}] {Name} - Score: {Score}";
}

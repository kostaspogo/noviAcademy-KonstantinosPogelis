namespace WorldRank;

public class InMemoryPlayerRepository : IPlayerRepository
{
    private readonly List<Player> _players = new List<Player>();

    public void AddPlayer(Player p) =>
        _players.Add(p);


    public Player? FindPlayer(int playerId)=>
         _players.FirstOrDefault(p => p.Id == playerId);


    public bool DeletePlayer(int playerId)
    {
        var player = FindPlayer(playerId);
        return player is not null && _players.Remove(player);
    }

    public IEnumerable<IGrouping<int, Player>> GroupPlayersByScore()=>
         _players.GroupBy(p => p.Score);

}

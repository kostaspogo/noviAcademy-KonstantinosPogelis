namespace WorldRank;

public interface IPlayerRepository

{
    void AddPlayer(Player p);
    Player? FindPlayer (int playerId);
    bool DeletePlayer(int playerId);
    IEnumerable<IGrouping<int, Player>> GroupPlayersByScore();
}

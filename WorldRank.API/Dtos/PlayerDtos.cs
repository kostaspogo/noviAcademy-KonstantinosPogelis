using WorldRank.Domain;

namespace WorldRank.API.Dtos
{
    public record CreatePlayerRequest(string Name, int Score);

    public record PlayerResponse(int Id, string Name, int Score)
    {
        public static PlayerResponse From(Player player) =>
            new(player.Id, player.Name, player.Score);
    }
}

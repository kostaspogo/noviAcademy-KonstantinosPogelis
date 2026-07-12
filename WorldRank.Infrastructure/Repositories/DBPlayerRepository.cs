using NLog;
using WorldRank.Application.Interfaces;
using WorldRank.Domain;
using WorldRank.Infrastructure.Persistence.Context;

namespace WorldRank.Infrastructure.Repositories
{
    // Database-backed υλοποίηση του IPlayerRepository μέσω Entity Framework Core.
    // Ίδια συμπεριφορά/queries με το InMemoryPlayerRepository, αλλά μόνιμη αποθήκευση στη βάση.
    public class DBPlayerRepository : IPlayerRepository
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly WorldRankDbContext _context;

        public DBPlayerRepository(WorldRankDbContext context)
        {
            _context = context;
        }

        public void AddPlayer(Player player)
        {
            _context.Player.Add(player);
            _context.SaveChanges();
            _logger.Info("Player {PlayerId} ({Name}) added with score {Score}", player.Id, player.Name, player.Score);
        }

        public IEnumerable<Player> GetAllPlayers()
        {
            return _context.Player.ToList();
        }

        public void DeletePlayer(int playerId)
        {
            var player = _context.Player.FirstOrDefault(item => item.Id == playerId);

            if (player is null)
            {
                _logger.Warn("Delete skipped: player {PlayerId} not found", playerId);
                return;
            }

            _context.Player.Remove(player);
            _context.SaveChanges();
            _logger.Info("Player {PlayerId} deleted", playerId);
        }

        public Player? FindPlayer(int playerId)
        {
            return _context.Player.FirstOrDefault(item => item.Id == playerId);
        }

        public IEnumerable<IGrouping<int, Player>> GroupPlayersByScore()
        {
            // Το grouping γίνεται client-side (AsEnumerable) γιατί το EF δεν μεταφράζει IGrouping σε SQL.
            return _context.Player
                .AsEnumerable()
                .GroupBy(player => player.Score)
                .OrderByDescending(group => group.Key);
        }
    }
}

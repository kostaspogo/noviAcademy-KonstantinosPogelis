using Microsoft.EntityFrameworkCore;
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

        public async Task AddPlayerAsync(Player player, CancellationToken ct = default)
        {
            _context.Player.Add(player);
            await _context.SaveChangesAsync(ct);
            _logger.Info("Player {PlayerId} ({Name}) added with score {Score}", player.Id, player.Name, player.Score);
        }

        public async Task<IReadOnlyList<Player>> GetAllPlayersAsync(CancellationToken ct = default)
        {
            return await _context.Player.ToListAsync(ct);
        }

        public async Task DeletePlayerAsync(int playerId, CancellationToken ct = default)
        {
            var player = await _context.Player.FirstOrDefaultAsync(item => item.Id == playerId, ct);

            if (player is null)
            {
                _logger.Warn("Delete skipped: player {PlayerId} not found", playerId);
                return;
            }

            _context.Player.Remove(player);
            await _context.SaveChangesAsync(ct);
            _logger.Info("Player {PlayerId} deleted", playerId);
        }

        public async Task<Player?> FindPlayerAsync(int playerId, CancellationToken ct = default)
        {
            return await _context.Player.FirstOrDefaultAsync(item => item.Id == playerId, ct);
        }

        public async Task<IEnumerable<IGrouping<int, Player>>> GroupPlayersByScoreAsync(CancellationToken ct = default)
        {
            // Το grouping γίνεται client-side (AsEnumerable) γιατί το EF δεν μεταφράζει IGrouping σε SQL.
            var players = await _context.Player.ToListAsync(ct);
            return players
                .GroupBy(player => player.Score)
                .OrderByDescending(group => group.Key);
        }
    }
}

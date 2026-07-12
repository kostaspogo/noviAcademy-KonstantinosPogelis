using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WorldRank.Infrastructure.Persistence.Context
{
    // Χρησιμοποιείται ΜΟΝΟ σε design-time (dotnet ef migrations / database update).
    // Λέει στο EF πώς να φτιάξει τον DbContext εκτός εφαρμογής.
    public class WorldRankDbContextFactory : IDesignTimeDbContextFactory<WorldRankDbContext>
    {
        public WorldRankDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<WorldRankDbContext>()
                .UseSqlServer("Server=localhost,1433;Database=NoviAcademyCodeFirst;User Id=sa;Password=Novibet!2025;TrustServerCertificate=true")
                .Options;

            return new WorldRankDbContext(options);
        }
    }
}

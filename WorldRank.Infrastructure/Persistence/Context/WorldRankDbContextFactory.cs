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
            // Mac / Docker SQL Server (sa auth). Στα Windows: Server=localhost;...;Integrated Security=true;TrustServerCertificate=true
            var options = new DbContextOptionsBuilder<WorldRankDbContext>()
                .UseSqlServer("Server=localhost;Database=NoviAcademyCodeFirst;Integrated Security=true;TrustServerCertificate=true")
                .Options;

            return new WorldRankDbContext(options);
        }
    }
}

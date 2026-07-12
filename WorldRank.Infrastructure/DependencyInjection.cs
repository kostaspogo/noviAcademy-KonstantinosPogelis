using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WorldRank.Application.Interfaces;
using WorldRank.Infrastructure.Persistence.Context;
using WorldRank.Infrastructure.Repositories;

namespace WorldRank.Infrastructure
{
    public static class DependencyInjection
    {
        // Ο "διακόπτης": useDatabase=false -> InMemory (default), true -> EF Core / SQL Server.
        // Ο κώδικας που καταναλώνει τα IPlayerRepository/IWalletRepository ΔΕΝ αλλάζει καθόλου.
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, bool useDatabase = false)
        {
            if (useDatabase)
            {
                services.AddDbContext<WorldRankDbContext>(options =>
                    options.UseSqlServer("Server=localhost,1433;Database=NoviAcademyCodeFirst;User Id=sa;Password=Novibet!2025;TrustServerCertificate=true"));

                // Τα DB repos εξαρτώνται από τον DbContext (Scoped) -> Scoped.
                services.AddScoped<IPlayerRepository, DBPlayerRepository>();
                services.AddScoped<IWalletRepository, DBWalletRepository>();
            }
            else
            {
                // Τα InMemory repos κρατούν state στη μνήμη -> Singleton (ένα για όλη την εφαρμογή).
                services.AddSingleton<IPlayerRepository, InMemoryPlayerRepository>();
                services.AddSingleton<IWalletRepository, InMemoryWalletRepository>();
            }

            return services;
        }
    }
}

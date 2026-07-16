using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using WorldRank.Domain;

namespace WorldRank.Infrastructure.Persistence.Context
{
    public partial class WorldRankDbContext : DbContext
    {
        public DbSet<Player> Player { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<CurrencyRates> CurrencyRates { get; set; }

        public WorldRankDbContext(DbContextOptions<WorldRankDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>(x =>
            {
                x.ToTable("Players");
                x.HasKey(y => y.Id);
                x.Property(y => y.Id).ValueGeneratedNever();
                x.Property(y => y.Name).HasMaxLength(100).IsRequired();
                x.Property(y => y.Score).IsRequired();
            });

            modelBuilder.Entity<Wallet>(x =>
            {
                x.ToTable("Wallets");
                x.HasKey(y => y.Id);
                // Το Id είναι identity (το παράγει η βάση) — δεν βάζουμε ValueGeneratedNever.
                x.Property(y => y.PlayerId).IsRequired();
                x.Property(y => y.Currency).IsRequired();
                x.Property(y => y.Balance).HasColumnType("decimal(18,2)").IsRequired();
                x.Property(y => y.IsBlocked).IsRequired();
                // Ένα wallet ανά (player, currency) — ο ίδιος κανόνας με το InMemory.
                x.HasIndex(y => new { y.PlayerId, y.Currency }).IsUnique();
            });

            modelBuilder.Entity<CurrencyRates>(x =>
            {
                x.ToTable("CurrencyRates");
                // Σύνθετο κλειδί: μία ισοτιμία ανά (νόμισμα, ημερομηνία).
                x.HasKey(y => new { y.Currency, y.Date });
                x.Property(y => y.Currency).HasMaxLength(3).IsRequired();
                x.Property(y => y.Rate).HasColumnType("decimal(18,6)").IsRequired();
                x.Property(y => y.Date).IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}

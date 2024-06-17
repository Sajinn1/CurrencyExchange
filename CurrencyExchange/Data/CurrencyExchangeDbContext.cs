

using CurrencyExchange.Entities;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchange.Data
{
    public class CurrencyExchangeDbContext : DbContext
    {
        public CurrencyExchangeDbContext(DbContextOptions<CurrencyExchangeDbContext> options) : base(options)
        {

        }
        public DbSet<Currency> Currencies { get; set; }

        public DbSet<ExchangeRate> ExchangeRates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ExchangeRate>(entity =>
            {
                entity.Property(e => e.CommiPer)
                      .HasColumnType("decimal(18, 2)");

                modelBuilder.Entity<ExchangeRate>()
                .HasOne(e => e.Currency)
                .WithMany() // Specify the relationship if needed (e.g., WithMany(c => c.ExchangeRates))
                .HasForeignKey(e => e.CurrencyID);
            });

        }

    }
}


using LootCamel.Models;
using Microsoft.EntityFrameworkCore;

namespace LootCamel.Data
{
    public class LootCamelContext : DbContext
    {
        public DbSet<LootPlayer> LootPlayers { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<LootItem> LootItems { get; set; }

        public LootCamelContext(DbContextOptions<LootCamelContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LootPlayer>()
                .HasIndex(u => u.Nickname)
                .IsUnique();

            modelBuilder.Entity<LootItem>()
                .HasIndex(i => i.ItemName)
                .IsUnique();
        }

    }
}

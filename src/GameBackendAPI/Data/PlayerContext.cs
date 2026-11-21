using GameBackendAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameBackendAPI.Data
{
    public class PlayerContext(DbContextOptions<PlayerContext> dbContextOptions) : DbContext(dbContextOptions)
    {
        public DbSet<Player> Players => Set<Player>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Player>().Property(p => p.Username).HasMaxLength(50);
            modelBuilder.Entity<Player>().HasIndex(p => p.Username).IsUnique();
            modelBuilder.Entity<Player>().Property(p => p.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}

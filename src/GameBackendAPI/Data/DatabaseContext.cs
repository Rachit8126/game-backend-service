using GameBackendAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameBackendAPI.Data
{
    public class DatabaseContext(DbContextOptions<DatabaseContext> dbContextOptions) : DbContext(dbContextOptions)
    {
        public DbSet<PlayerEntity> Players => Set<PlayerEntity>();
        //public DbSet<ScoreEntity> Scores => Set<ScoreEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasPostgresExtension("citext");

            modelBuilder.Entity<PlayerEntity>().Property(p => p.Username).HasMaxLength(50);
            modelBuilder.Entity<PlayerEntity>().Property(p => p.Username).HasColumnType("citext");
            modelBuilder.Entity<PlayerEntity>().Property(p => p.Email).HasMaxLength(254);
            modelBuilder.Entity<PlayerEntity>().Property(p => p.HashedPassword).HasMaxLength(150);
            modelBuilder.Entity<PlayerEntity>().Property(p => p.Email).HasColumnType("citext");
            modelBuilder.Entity<PlayerEntity>().HasIndex(p => p.Username).IsUnique();
            modelBuilder.Entity<PlayerEntity>().HasIndex(p => p.Email).IsUnique();
            modelBuilder.Entity<PlayerEntity>().Property(p => p.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            //modelBuilder.Entity<ScoreEntity>().Property(p => p.SubmissionTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            //modelBuilder.Entity<ScoreEntity>().HasIndex(p => p.SubmissionTime);
            //modelBuilder.Entity<ScoreEntity>().HasOne<PlayerEntity>().WithMany().HasForeignKey(p => p.PlayerId);
        }
    }
}

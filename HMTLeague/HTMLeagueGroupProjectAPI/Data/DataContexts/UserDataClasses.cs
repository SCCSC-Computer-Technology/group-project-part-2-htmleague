using HTMLeagueGroupProjectAPI.Data.DataModels;
using Microsoft.EntityFrameworkCore;

namespace SportStatsWeb.Data
{
    public class UsersDbContext : DbContext
    {
        public UsersDbContext(DbContextOptions<UsersDbContext> options)
            : base(options)
        {
        }

        // User authentication tables
        public DbSet<UserCredential> UserCredentials { get; set; }
        public DbSet<UserPreference> UserPreferences { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserCredential>(entity =>
            {
                entity.ToTable("UserCredential", "dbo");
                entity.HasKey(e => e.Email);
                entity.Property(e => e.Email).HasMaxLength(320).IsRequired();
                entity.Property(e => e.HashedPassword).HasColumnType("BINARY(32)").IsRequired();
                entity.Property(e => e.Salt).HasColumnType("CHAR(36)").IsRequired();

                entity.HasOne(e => e.Preference)
                      .WithOne(p => p.Credential)
                      .HasForeignKey<UserPreference>(p => p.Email);
            });

            modelBuilder.Entity<UserPreference>(entity =>
            {
                entity.ToTable("UserPreference", "dbo");
                entity.HasKey(e => e.Email);
                entity.Property(e => e.Email).HasMaxLength(320).IsRequired();
                entity.Property(e => e.Font).HasMaxLength(80).HasDefaultValue("Default");
                entity.Property(e => e.Theme).HasDefaultValue(0);
                entity.Property(e => e.EnabledSports).HasMaxLength(50);
            });
        }

    }
}
using CentaurScores.Model;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.Configuration;

namespace CentaurScores.Persistence
{
    // In the project folder:
    // - dotnet ef migrations add MigrationName
    // - dotnet dotnet ef migrations script 

    /// <summary>
    /// Database context for CentaurScores
    /// </summary>
    /// <remarks>Constructor</remarks>
    public class CentaurScoresDbContext(IConfiguration configuration) : DbContext
    {
        /// <summary>
        /// Accounts
        /// </summary>
        public DbSet<AccountEntity> Accounts { get; set; }
        /// <summary>
        /// All ACLs
        /// </summary>
        public DbSet<AclEntity> ACLs { get; set; }
        /// <summary>
        /// All matches
        /// </summary>
        public DbSet<MatchEntity> Matches { get; set; }
        /// <summary>
        /// All participants per match.
        /// </summary>
        public DbSet<ParticipantEntity> Participants { get; set; }
        /// <summary>
        /// Global settings.
        /// </summary>
        public DbSet<CsSetting> Settings { get; set; }
        /// <summary>
        /// ALl participant lists.
        /// </summary>
        public DbSet<ParticipantListEntity> ParticipantLists { get; set; }
        /// <summary>
        /// Members for each participant list.
        /// </summary>
        public DbSet<ParticipantListEntryEntity> ParticipantListEntries { get; set; }
        /// <summary>
        /// All competitions.
        /// </summary>
        public DbSet<CompetitionEntity> Competitions { get; set; }

        /// <inheritdoc/>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string? connectionString = configuration.GetConnectionString("CentaurScoresDatabase");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Please configure the CentaurScoresDatabase connection string.");
            }
            optionsBuilder.UseMySQL(connectionString);
        }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MatchEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.MatchCode).IsRequired();
                entity.Property(e => e.MatchName).IsRequired();
                entity.HasMany(e => e.Participants).WithOne(p => p.Match).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ParticipantEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.DeviceID).IsRequired();
                entity.Property(e => e.Lijn).IsRequired();
                entity.Property(e => e.EndsJSON).IsRequired();
            });

            modelBuilder.Entity<CsSetting>(entity =>
            {
                entity.HasKey(e => e.Name);
            });

            modelBuilder.Entity<ParticipantListEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired();
                entity.HasMany(e => e.Entries).WithOne(p => p.List).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ParticipantListEntryEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<CompetitionEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired();
                // optional one-to-many
                entity.HasOne(e => e.ParticipantList).WithMany(p => p.Competitions).OnDelete(DeleteBehavior.NoAction).IsRequired(false);
                // optional meny-to-one
                entity.HasMany(e => e.Matches).WithOne(m => m.Competition).OnDelete(DeleteBehavior.Cascade).IsRequired(false);
            });

            modelBuilder.Entity<AccountEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Username).IsRequired().HasMaxLength(32);
                entity.HasIndex(e => e.Username).IsUnique(true);
                entity.Property(e => e.SaltedPasswordHash).IsRequired().HasMaxLength(72); // 64 plus a hash
                entity.HasMany(e => e.ACLs).WithMany(a => a.Accounts);
            });

            modelBuilder.Entity<AclEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(256);
                entity.HasIndex(e => e.Name).IsUnique(true);
                entity.HasMany(e => e.Accounts).WithMany(a => a.ACLs);
            });
        }
    }
}

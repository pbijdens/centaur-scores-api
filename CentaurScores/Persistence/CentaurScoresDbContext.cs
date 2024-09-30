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
    //
    // Create the new migrations bundle using this command
    // - dotnet ef migrations bundle
    public class CentaurScoresDbContext : DbContext
    {
        private readonly IConfiguration configuration;

        public DbSet<MatchEntity> Matches { get; set; }
        public DbSet<ParticipantEntity> Participants { get; set; }
        public DbSet<CsSetting> Settings { get; set; }
        public DbSet<ParticipantListEntity> ParticipantLists { get; set; }
        public DbSet<ParticipantListEntryEntity> ParticipantListEntries { get; set; }

        public CentaurScoresDbContext(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string? connectionString = configuration.GetConnectionString("CentaurScoresDatabase");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Please configure the CentaurScoresDatabase connection string.");
            }
            optionsBuilder.UseMySQL(connectionString);
        }

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
        }
    }
}

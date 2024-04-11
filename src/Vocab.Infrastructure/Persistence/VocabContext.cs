using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vocab.Application.Configuration;
using Vocab.Core.Entities;

namespace Vocab.Infrastructure.Persistence
{
    public class VocabContext(IOptions<PostgresConfiguration> options, ILoggerFactory loggerFactory) : DbContext
    {
        private readonly PostgresConfiguration _configuration = options.Value ?? throw new ArgumentNullException(nameof(options), "Параметры подключения к БД не получены.");
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(loggerFactory);
            optionsBuilder.EnableSensitiveDataLogging(sensitiveDataLoggingEnabled: _configuration.SensitiveDataLoggingEnabled);
            optionsBuilder.UseNpgsql(_configuration.ConnectionString, options => options.MigrationsAssembly("Vocab.WebApi"));
        }

        public DbSet<StatementPair> StatementPairs => Set<StatementPair>();
        public DbSet<StatementDictionary> StatementDictionaries => Set<StatementDictionary>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StatementPair>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.StatementsDictionary).WithMany(e => e.StatementPairs).HasForeignKey(e => e.RelatedDictionaryId).OnDelete(DeleteBehavior.Restrict).IsRequired();

                entity.ToTable(t => t.HasCheckConstraint("GuessingLevelCheck", $"\"GuessingLevel\" >= {StatementPair.MIN_GUESSING_LEVEL} AND \"GuessingLevel\" <= {StatementPair.MAX_GUESSING_LEVEL}"));
            });

            modelBuilder.Entity<StatementDictionary>(e =>
            {
                e.HasKey(e => e.Id);
            });
        }
    }
}

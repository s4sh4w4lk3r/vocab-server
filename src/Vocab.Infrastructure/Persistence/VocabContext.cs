using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using Vocab.Application.Configuration;
using Vocab.Application.Shared;
using Vocab.Core.Entities;
using static Vocab.Application.Shared.ResultMessages;

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

        public async Task<Result> TrySaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await SaveChangesAsync(cancellationToken) != 0 ? Result.Ok("Операция в базе данных прошла успешно.") : Result.Fail(NotFound);
            }

            catch (DbUpdateConcurrencyException)
            {
                return Result.Fail(NotFound);
            }

            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == PostgresErrorCodes.ForeignKeyViolation)
            {
                return Result.Fail($"{ForeignKeyError} Имя ограничения: {pgEx.ConstraintName}");
            }

            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                return Result.Fail($"{UniqueIndexError} Имя ограничения: {pgEx.ConstraintName}");
            }

            catch
            {
                throw;
            }
        }
    }
}

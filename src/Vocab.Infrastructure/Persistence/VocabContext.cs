using Microsoft.EntityFrameworkCore;
using Npgsql;
using Vocab.Application.ValueObjects;
using Vocab.Core.Entities;
using static Vocab.Application.Constants.ResultMessages;

namespace Vocab.Infrastructure.Persistence
{
    public class VocabContext(DbContextOptions<VocabContext> options) : DbContext(options)
    {
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

        public async Task<ResultVocab> TrySaveChangesAsync(string successMessage = "Операция в базе данных прошла успешно.", CancellationToken cancellationToken = default)
        {
            try
            {
                return await SaveChangesAsync(cancellationToken) != 0 ? ResultVocab.Ok(successMessage) : ResultVocab.Fail(NotFound);
            }

            catch (DbUpdateConcurrencyException)
            {
                return ResultVocab.Fail(NotFound);
            }

            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == PostgresErrorCodes.ForeignKeyViolation)
            {
                return ResultVocab.Fail($"{ForeignKeyError} Имя ограничения: {pgEx.ConstraintName}");
            }

            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                return ResultVocab.Fail($"{UniqueIndexError} Имя ограничения: {pgEx.ConstraintName}");
            }

            catch
            {
                throw;
            }
        }
    }
}

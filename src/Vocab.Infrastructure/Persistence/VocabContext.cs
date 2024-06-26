﻿using Microsoft.EntityFrameworkCore;
using Vocab.Application.ValueObjects.Result;
using Vocab.Core.Entities;

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
                entity.HasOne(e => e.StatementsDictionary).WithMany(e => e.StatementPairs).HasForeignKey(e => e.StatementsDictionaryId).OnDelete(DeleteBehavior.Cascade).IsRequired();
                entity.Property(e => e.GuessingLevel).HasDefaultValue(StatementPair.MIN_GUESSING_LEVEL);

                entity.Property(x => x.Source).HasMaxLength(StatementPair.MAX_SOURCE_LENGTH);
                entity.Property(x => x.Target).HasMaxLength(StatementPair.MAX_TARGET_LENGTH);

            });

            modelBuilder.Entity<StatementDictionary>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(x => x.Name).HasMaxLength(StatementDictionary.MAX_NAME_LENGTH);

            });
        }

        public async Task<ResultVocab<int>> TrySaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return ResultVocab.Success().AddValue(await SaveChangesAsync(cancellationToken));
            }

            /*catch (DbUpdateConcurrencyException)
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
            }*/
            catch
            {
                throw;
            }
        }
    }
}

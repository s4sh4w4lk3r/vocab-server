using Microsoft.EntityFrameworkCore;
using Throw;
using Vocab.Application.Abstractions.Services;
using Vocab.Application.Validators;
using Vocab.Application.ValueObjects.Result;
using Vocab.Application.ValueObjects.Result.Errors;
using Vocab.Core.Entities;
using Vocab.Core.Enums;
using Vocab.Infrastructure.Persistence;

namespace Vocab.Infrastructure.Services
{
    public class StatementPairService(VocabContext context) : IStatementPairService
    {
        public async Task<ResultVocab<long>> Add(Guid userId, StatementPair statementPair)
        {
            userId.Throw().IfDefault();
            statementPair.ThrowIfNull();

            var valResult = new StatementPairValidator(willBeInserted: true).Validate(statementPair);
            if (!valResult.IsValid)
            {
                return ResultVocab.Failure(StatementPairErrors.Validation(valResult)).AddValue<long>(default);
            }

            if (await context.StatementDictionaries.AnyAsync(sd => sd.OwnerId == userId && sd.Id == statementPair.StatementsDictionaryId) is false)
            {
                return ResultVocab.Failure(StatementDictionaryErrors.NotFound).AddValue<long>(default);
            }

            await context.StatementPairs.AddAsync(statementPair);
            return (await context.TrySaveChangesAsync()).AddValue(statementPair.Id);
        }

        public async Task<ResultVocab> Delete(Guid userId, long statementPairId)
        {
            userId.Throw().IfDefault();
            statementPairId.Throw().IfDefault();

            int rowsDeleted = await context.StatementPairs
                .Where(sp => sp.StatementsDictionary!.OwnerId == userId && sp.Id == statementPairId)
                .ExecuteDeleteAsync();

            return rowsDeleted == 1 ? ResultVocab.Success() : ResultVocab.Failure(StatementPairErrors.NotFound);
        }

        public Task<ResultVocab> SetSource(Guid userId, long statementPairId, string source)
        {
            userId.Throw().IfDefault();
            statementPairId.Throw().IfDefault();
            source.ThrowIfNull().IfEmpty().IfWhiteSpace();

            return UpdateStatementPair(userId, statementPairId, x => x.Source = source);
        }

        public Task<ResultVocab> SetTarget(Guid userId, long statementPairId, string target)
        {
            userId.Throw().IfDefault();
            statementPairId.Throw().IfDefault();
            target.ThrowIfNull().IfEmpty().IfWhiteSpace();

            return UpdateStatementPair(userId, statementPairId, x => x.Target = target);
        }

        public Task<ResultVocab> SetCategory(Guid userId, long statementPairId, StatementCategory category)
        {
            userId.Throw().IfDefault();
            statementPairId.Throw().IfDefault();
            category.Throw().IfOutOfRange();

            return UpdateStatementPair(userId, statementPairId, x => x.StatementCategory = category);
        }

        public async Task<ResultVocab<StatementPair>> GetById(Guid userId, long statementPairId)
        {
            userId.Throw().IfDefault();
            statementPairId.Throw().IfDefault();

            StatementPair? statementPair = await context.StatementPairs
                .SingleOrDefaultAsync(x => x.Id == statementPairId && x.StatementsDictionary!.OwnerId == userId);

            return statementPair is not null ? ResultVocab.Success().AddValue(statementPair) : ResultVocab.Failure(StatementPairErrors.NotFound).AddValue<StatementPair>(default);
        }

        public async Task<ResultVocab<StatementPair[]>> GetStatements(Guid userId, long dictionaryId, int offset)
        {
            userId.Throw().IfDefault();
            dictionaryId.Throw().IfDefault();
            offset.Throw().IfNegative();

            const int STATEMENTS_LIMIT = 100;

            StatementPair[] statementPairs = await context.StatementPairs
                .AsNoTracking()
                .Where(x => x.StatementsDictionaryId == dictionaryId && x.StatementsDictionary!.OwnerId == userId)
                .OrderBy(x => x.Source).Skip(offset).Take(STATEMENTS_LIMIT).ToArrayAsync();

            return statementPairs.LongLength > 0 ? ResultVocab.Success().AddValue(statementPairs) : ResultVocab.Failure(StatementPairErrors.NotFound).AddValue<StatementPair[]>(default);
        }

        private async Task<ResultVocab> UpdateStatementPair(Guid userId, long statementPairId, Action<StatementPair> action)
        {
            StatementPair? statement = await context.StatementPairs
                .Where(sp => sp.StatementsDictionary!.OwnerId == userId && sp.Id == statementPairId)
                .Include(x => x.StatementsDictionary)
                .SingleOrDefaultAsync();

            if (statement is null || statement.StatementsDictionary is null)
            {
                return ResultVocab.Failure(StatementPairErrors.NotFound);
            }

            DateTime utcNow = DateTime.UtcNow;
            statement.LastModified = utcNow;
            statement.StatementsDictionary.LastModified = utcNow;

            action(statement);

            return await context.TrySaveChangesAsync();
        }
    }
}

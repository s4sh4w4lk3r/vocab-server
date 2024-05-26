using Microsoft.EntityFrameworkCore;
using Throw;
using Vocab.Application.Abstractions.Services;
using Vocab.Application.Constants;
using Vocab.Application.Validators;
using Vocab.Application.ValueObjects;
using Vocab.Core.Entities;
using Vocab.Core.Enums;
using Vocab.Infrastructure.Persistence;

namespace Vocab.Infrastructure.Services
{
    public class StatementPairService(VocabContext context) : IStatementPairService
    {
        public async Task<ResultVocab<StatementPair>> Insert(Guid userId, StatementPair statementPair)
        {
            userId.Throw().IfDefault();
            statementPair.ThrowIfNull();

            var valResult = new StatementPairValidator(willBeInserted: true).Validate(statementPair);
            if (!valResult.IsValid)
            {
                return ResultVocab.Fail(valResult.ToString())
                    .AddValue(default(StatementPair));
            }

            if (await context.StatementDictionaries.AnyAsync(sd => sd.OwnerId == userId && sd.Id == statementPair.StatementsDictionaryId))
            {
                return ResultVocab.Fail("userId не соответствует ownerId или указанный словарь не найден.").AddValue(default(StatementPair));
            }

            await context.StatementPairs.AddAsync(statementPair);
            return (await context.TrySaveChangesAsync(ResultMessages.Added)).AddValue(statementPair);
        }

        public async Task<ResultVocab> Delete(Guid userId, long statementPairId)
        {
            userId.Throw().IfDefault();
            statementPairId.Throw().IfDefault();

            int rowsDeleted = await context.StatementPairs
                .Where(sp => sp.StatementsDictionary!.OwnerId == userId && sp.Id == statementPairId).ExecuteDeleteAsync();
            return rowsDeleted == 1 ? ResultVocab.Ok(ResultMessages.Deleted) : ResultVocab.Fail(ResultMessages.NotFound);
        }

        public async Task<ResultVocab> SetSource(Guid userId, long statementPairId, string source)
        {
            userId.Throw().IfDefault();
            statementPairId.Throw().IfDefault();
            source.ThrowIfNull().IfEmpty().IfWhiteSpace();

            return await ExecuteUpdateProperty(userId, statementPairId, x => x.Source, source);
        }

        public async Task<ResultVocab> SetTarget(Guid userId, long statementPairId, string target)
        {
            userId.Throw().IfDefault();
            statementPairId.Throw().IfDefault();
            target.ThrowIfNull().IfEmpty().IfWhiteSpace();

            return await ExecuteUpdateProperty(userId, statementPairId, x => x.Target, target);
        }

        public async Task<ResultVocab<StatementPair>> Update(Guid userId, StatementPair statementPair)
        {
            userId.Throw().IfDefault();
            statementPair.ThrowIfNull();

            if (!await context.StatementDictionaries.AnyAsync(sd=>sd.OwnerId == userId && sd.Id == statementPair.StatementsDictionaryId))
            {
                return ResultVocab.Fail("userId не соответствует ownerId или указанный словарь не найден.").AddValue(default(StatementPair));
            }

            context.StatementPairs.Update(statementPair);
            return (await context.TrySaveChangesAsync(ResultMessages.Added)).AddValue(statementPair);
        }
        public async Task<ResultVocab> SetCategory(Guid userId, long statementPairId, StatementCategory category)
        {
            userId.Throw().IfDefault();
            statementPairId.Throw().IfDefault();
            category.Throw().IfOutOfRange();

            return await ExecuteUpdateProperty(userId, statementPairId, x => x.StatementCategory, category);
        }

        private async Task<ResultVocab> ExecuteUpdateProperty<TProperty>(Guid userId, long statementPairId, Func<StatementPair, TProperty> func, TProperty value)
        {
            userId.Throw().IfDefault();
            statementPairId.Throw().IfDefault();
            func.ThrowIfNull();

            int rowsUpdated = await context.StatementPairs
                .Where(sp => sp.StatementsDictionary!.OwnerId == userId && sp.Id == statementPairId)
                .ExecuteUpdateAsync(sp => sp.SetProperty(func, value));
            return rowsUpdated == 1 ? ResultVocab.Ok(ResultMessages.Updated) : ResultVocab.Fail(ResultMessages.NotFound);
        }

        public async Task<ResultVocab<StatementPair>> GetById(Guid userId, long statementPairId)
        {
            userId.Throw().IfDefault();
            statementPairId.Throw().IfDefault();

            StatementPair? statementPair = await context.StatementPairs.SingleOrDefaultAsync(x => x.Id == statementPairId && x.StatementsDictionary!.OwnerId == userId);
            return statementPair is not null ? ResultVocab.Ok().AddValue(statementPair) : ResultVocab.Fail(ResultMessages.NotFound).AddValue(default(StatementPair));
        }

        public async Task<ResultVocab<StatementPair[]>> GetDictionaryStatementPairs(Guid userId, long dictionaryId, int offset)
        {
            userId.Throw().IfDefault();
            dictionaryId.Throw().IfDefault();
            offset.Throw().IfNegative();

            bool isDictionaryExists = await context.StatementDictionaries.AnyAsync(x => x.Id == dictionaryId && x.OwnerId == userId);
            if (isDictionaryExists is false)
            {
                return ResultVocab.Fail(ResultMessages.NotFound).AddValue(default(StatementPair[]));
            }

            StatementPair[] statementPairs = await context.StatementPairs
                .Where(x => x.StatementsDictionaryId == dictionaryId).Skip(offset).Take(150).OrderBy(x=>x.Source).ToArrayAsync();
            return ResultVocab.Ok().AddValue(statementPairs);
        }
    }
}

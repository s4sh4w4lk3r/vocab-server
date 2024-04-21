using Microsoft.EntityFrameworkCore;
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
            var valResult = new StatementPairValidator(willBeInserted: true).Validate(statementPair);
            if (!valResult.IsValid)
            {
                return ResultVocab.Fail(valResult.ToString())
                    .AddValue(default(StatementPair));
            }

            if (await context.StatementDictionaries.AnyAsync(sd => sd.OwnerId == userId && sd.Id == statementPair.RelatedDictionaryId))
            {
                return ResultVocab.Fail("userId не соответствует ownerId или указанный словарь не найден.").AddValue(default(StatementPair));
            }

            await context.StatementPairs.AddAsync(statementPair);
            return (await context.TrySaveChangesAsync(ResultMessages.Added)).AddValue(statementPair);
        }

        public async Task<ResultVocab> Delete(Guid userId, long statementPairId)
        {
            int rowsDeleted = await context.StatementPairs
                .Where(sp => sp.StatementsDictionary!.OwnerId == userId && sp.Id == statementPairId).ExecuteDeleteAsync();
            return rowsDeleted == 1 ? ResultVocab.Ok(ResultMessages.Deleted) : ResultVocab.Ok(ResultMessages.NotFound);
        }

        public async Task<ResultVocab> SetSource(Guid userId, long statementPairId, string source) 
            => await ExecuteUpdateProperty(userId, statementPairId, x => x.Source, source);

        public async Task<ResultVocab> SetTarget(Guid userId, long statementPairId, string target) 
            => await ExecuteUpdateProperty(userId, statementPairId, x => x.Target, target);

        public async Task<ResultVocab<StatementPair>> Update(Guid userId, StatementPair statementPair)
        {
            if (!await context.StatementDictionaries.AnyAsync(sd=>sd.OwnerId == userId && sd.Id == statementPair.RelatedDictionaryId))
            {
                return ResultVocab.Fail("userId не соответствует ownerId или указанный словарь не найден.").AddValue(default(StatementPair));
            }

            context.StatementPairs.Update(statementPair);
            return (await context.TrySaveChangesAsync(ResultMessages.Added)).AddValue(statementPair);
        }

        public async Task<ResultVocab<int>> Decrease(Guid userId, long statementPairId)
        {
            var statementPair = await context.StatementPairs
                .SingleOrDefaultAsync(sp => sp.StatementsDictionary!.OwnerId == userId && sp.Id == statementPairId);
            if (statementPair is null)
            {
                return ResultVocab.Fail("Выражение не найдено.").AddValue(-1);
            }

            if (statementPair.GuessingLevel == 1)
            {
                return ResultVocab.Fail("Не требуется уменьшать рейтинг.").AddValue(statementPair.GuessingLevel);
            }

            statementPair.DecreaseRating();
            return(await context.TrySaveChangesAsync(ResultMessages.Updated)).AddValue(statementPair.GuessingLevel);
        }

        public async Task<ResultVocab<int>> Increase(Guid userId, long statementPairId)
        {
            var statementPair = await context.StatementPairs
                .SingleOrDefaultAsync(sp => sp.StatementsDictionary!.OwnerId == userId && sp.Id == statementPairId);
            if (statementPair is null)
            {
                return ResultVocab.Fail("Выражение не найдено.").AddValue(-1);
            }

            if (statementPair.GuessingLevel == 5)
            {
                return ResultVocab.Fail("Не требуется увеличивать рейтинг.").AddValue(statementPair.GuessingLevel);
            }

            statementPair.IncreaseRating();
            return (await context.TrySaveChangesAsync(ResultMessages.Updated)).AddValue(statementPair.GuessingLevel);
        }

        public async Task<ResultVocab> SetCategory(Guid userId, long statementPairId, StatementCategory category)
        {
            if (!Enum.IsDefined(category))
            {
                return ResultVocab.Fail("Неправильно указана категория.").AddValue(default(StatementPair));
            }

            return await ExecuteUpdateProperty(userId, statementPairId, x => x.StatementCategory, category);
        }

        private async Task<ResultVocab> ExecuteUpdateProperty<TProperty>(Guid userId, long statementPairId, Func<StatementPair, TProperty> func, TProperty value)
        {
            int rowsUpdated = await context.StatementPairs
                .Where(sp => sp.StatementsDictionary!.OwnerId == userId && sp.Id == statementPairId)
                .ExecuteUpdateAsync(sp => sp.SetProperty(func, value));
            return rowsUpdated == 1 ? ResultVocab.Ok(ResultMessages.Updated) : ResultVocab.Ok(ResultMessages.UpdateError);
        }
    }
}

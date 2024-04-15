using Microsoft.EntityFrameworkCore;
using Vocab.Application.Abstractions.Services;
using Vocab.Application.Constants;
using Vocab.Application.ValueObjects;
using Vocab.Core.Entities;
using Vocab.Infrastructure.Persistence;

namespace Vocab.Infrastructure.Services
{
    public class StatementDictionaryService(VocabContext context) : IStatementDictionaryService
    {
        public async Task<ResultVocab> Delete(Guid userId, long dictionaryId)
        {
#warning проверить
            return await context.StatementDictionaries.Where(sd => sd.Id == dictionaryId && sd.OwnerId == userId).ExecuteDeleteAsync() == 1
               ? ResultVocab.Ok(ResultMessages.Deleted) : ResultVocab.Ok(ResultMessages.NotFound);
        }

        public async Task<ResultVocab<List<StatementPair>?>> GetStatementsForChallenge(long dictionaryId, Guid userId, int gameLength = 25)
        {
#warning проверить
            if (gameLength > 150 || gameLength < 5)
            {
                return ResultVocab.Fail("Количество слов для игры должно быть не меньше 5 и не более 150").AddValue<List<StatementPair>?>(null);
            }

            List<StatementPair> statementPairs = await context.StatementPairs
                .Where(sp => sp.StatementsDictionary!.Id == dictionaryId && sp.StatementsDictionary.OwnerId == userId)
                .OrderBy(sp => sp.GuessingLevel).Take(gameLength).ToListAsync() ?? [];
            return statementPairs is { Count: > 0 } ? ResultVocab.Ok().AddValue<List<StatementPair>?>(statementPairs) : ResultVocab.Fail("Словарь пустой.").AddValue<List<StatementPair>?>(default); ;
        }

        public Task<ResultVocab<IQueryable<StatementPair>>> GetStatementsForChallenge(Guid userId, long dictionaryId)
        {
            throw new NotImplementedException();
        }

        public Task<ResultVocab<StatementDictionary>> Insert(StatementDictionary dictionary)
        {

            throw new NotImplementedException();
        }

        public Task<ResultVocab<StatementDictionary>> SetName(Guid userId, long dictionaryId, string name)
        {
            throw new NotImplementedException();
        }

        public Task<ResultVocab<StatementDictionary>> Update(StatementDictionary dictionary)
        {
            throw new NotImplementedException();
        }
    }
}

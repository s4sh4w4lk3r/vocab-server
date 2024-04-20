﻿using Microsoft.EntityFrameworkCore;
using Vocab.Application.Abstractions.Services;
using Vocab.Application.Constants;
using Vocab.Application.Validators;
using Vocab.Application.ValueObjects;
using Vocab.Core.Entities;
using Vocab.Infrastructure.Persistence;

namespace Vocab.Infrastructure.Services
{
    public class StatementDictionaryService(VocabContext context) : IStatementDictionaryService
    {
#error переписать тесты
        public async Task<ResultVocab> Delete(Guid userId, long dictionaryId)
        {
            int rowsDeleted = await context.StatementDictionaries.Where(sd => sd.Id == dictionaryId && sd.OwnerId == userId).ExecuteDeleteAsync();
            return rowsDeleted == 1 ? ResultVocab.Ok(ResultMessages.Deleted) : ResultVocab.Ok(ResultMessages.NotFound);
        }

        public async Task<ResultVocab<List<StatementPair>>> GetStatementsForChallenge(Guid userId, long dictionaryId, int gameLength = 25)
        {
            if (gameLength > 150 || gameLength < 5)
            {
                return ResultVocab.Fail("Количество слов для игры должно быть не меньше 5 и не более 150.").AddValue(default(List<StatementPair>));
            }

            List<StatementPair> statementPairs = await context.StatementPairs
                .Where(sp => sp.StatementsDictionary!.Id == dictionaryId && sp.StatementsDictionary.OwnerId == userId)
                .OrderBy(sp => sp.GuessingLevel).Take(count: gameLength).ToListAsync();
            return statementPairs is { Count: > 0 } ? ResultVocab.Ok().AddValue(statementPairs) : ResultVocab.Fail("Словарь пустой.").AddValue(default(List<StatementPair>)); ;
        }

        public async Task<ResultVocab<StatementDictionary>> Insert(Guid userId, StatementDictionary dictionary)
        {
            var valResult = new StatementDictionaryValidator(willBeInserted: true).Validate(dictionary);
            if (!valResult.IsValid)
            {
                return ResultVocab.Fail(valResult.ToString()).AddValue(default(StatementDictionary));
            }

            if (userId != dictionary.OwnerId)
            {
                return ResultVocab.Fail("userId не соответствует ownerId.").AddValue(default(StatementDictionary));
            }

            await context.StatementDictionaries.AddAsync(dictionary);
            return (await context.TrySaveChangesAsync(ResultMessages.Added)).AddValue(dictionary);
        }

        public async Task<ResultVocab> SetName(Guid userId, long dictionaryId, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return ResultVocab.Fail("Имя не должно быть пустым.");
            }
            int rowsUpdated = await context.StatementDictionaries.Where(sd => sd.OwnerId == userId && sd.Id == dictionaryId).ExecuteUpdateAsync(sd => sd.SetProperty(p => p.Name, name));
            return rowsUpdated == 1 ? ResultVocab.Ok(ResultMessages.Updated) : ResultVocab.Ok(ResultMessages.UpdateError);
        }

        public async Task<ResultVocab<StatementDictionary>> Update(Guid userId, StatementDictionary dictionary)
        {
            var valResult = new StatementDictionaryValidator(willBeInserted: false).Validate(dictionary);
            if (!valResult.IsValid)
            {
                return ResultVocab.Fail(valResult.ToString()).AddValue(default(StatementDictionary));
            }

            if (dictionary.OwnerId != userId)
            {
                return ResultVocab.Fail("userId не соответствует ownerId.").AddValue(default(StatementDictionary));
            }

            if (!await context.StatementDictionaries.AnyAsync(sd => sd.OwnerId == userId && sd.Id == dictionary.Id))
            {
                return ResultVocab.Fail("Словарь для обновления не найден.").AddValue(default(StatementDictionary));
            }    

            context.StatementDictionaries.Update(dictionary);
            return (await context.TrySaveChangesAsync(ResultMessages.Updated)).AddValue(dictionary);
        }
    }
}

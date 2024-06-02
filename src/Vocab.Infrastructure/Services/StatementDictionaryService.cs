using Microsoft.EntityFrameworkCore;
using Throw;
using Vocab.Application.Abstractions.Services;
using Vocab.Application.Constants;
using Vocab.Application.Types;
using Vocab.Application.Validators;
using Vocab.Application.ValueObjects;
using Vocab.Core.Entities;
using Vocab.Infrastructure.Persistence;

namespace Vocab.Infrastructure.Services
{
    public class StatementDictionaryService(VocabContext context) : IStatementDictionaryService
    {
        public async Task<ResultVocab> Delete(Guid userId, long dictionaryId)
        {
            userId.Throw().IfDefault();
            dictionaryId.Throw().IfDefault();

            int rowsDeleted = await context.StatementDictionaries.Where(sd => sd.Id == dictionaryId && sd.OwnerId == userId).ExecuteDeleteAsync();
            return rowsDeleted == 1 ? ResultVocab.Ok(ResultMessages.Deleted) : ResultVocab.Fail(ResultMessages.NotFound);
        }

        public async Task<ResultVocab<StatementDictionary>> GetById(Guid userId, long dictionaryId)
        {
            userId.Throw().IfDefault();
            dictionaryId.Throw().IfDefault();

            StatementDictionary? statementDictionary = await context.StatementDictionaries.AsNoTracking().SingleOrDefaultAsync(x=>x.Id == dictionaryId && x.OwnerId == userId);
            return statementDictionary is not null ? ResultVocab.Ok().AddValue(statementDictionary) : ResultVocab.Fail(ResultMessages.NotFound).AddValue(default(StatementDictionary));
        }

        public async Task<ResultVocab<StatementDictionary[]>> GetUserDictionaries(Guid userId, bool appendSomeTopStatements, int offset)
        {
            userId.Throw().IfDefault();
            offset.Throw().IfNegative();

            const int NUMBER_OF_DICTIONARIES = 15;
            const int NUMBER_OF_TOP_STATEMENTS = 15;

            StatementDictionary[] statementDictionaries;

            if (appendSomeTopStatements)
            {
                statementDictionaries = await context.StatementDictionaries.AsNoTracking().Where(x => x.OwnerId == userId)
                 .OrderByDescending(x => x.PositionPriority).ThenBy(x => x.Name).Skip(offset).Take(NUMBER_OF_DICTIONARIES).
                 Include(x=>x.StatementPairs.OrderBy(z => z.Source).Take(NUMBER_OF_TOP_STATEMENTS))
                 .ToArrayAsync();
            }
            else
            {
                statementDictionaries = await context.StatementDictionaries.AsNoTracking().Where(x => x.OwnerId == userId)
                .OrderByDescending(x => x.PositionPriority).ThenBy(x=>x.Name).Skip(offset).Take(NUMBER_OF_DICTIONARIES).ToArrayAsync();
            }

            return ResultVocab.Ok().AddValue(statementDictionaries);
        }

        public async Task<ResultVocab<ImportStatementsResult>> ImportStatements(Guid userId, long dictionaryId, Stream stream, string separator)
        {
            userId.Throw().IfDefault();
            dictionaryId.Throw().IfDefault();
            stream.ThrowIfNull();

            if (!await context.StatementDictionaries.AnyAsync(x => x.OwnerId == userId && x.Id == dictionaryId))
            {
                return ResultVocab.Fail("Словарь не найден.").AddValue(default(ImportStatementsResult));
            }

            ParseStatementsHelper importStatementsHelper = new(stream, dictionaryId, separator);
            var helperResult = await importStatementsHelper.ParseDocument();


            if (helperResult.Success is false || helperResult.Value is null)
            {
                return ResultVocab.Fail("Ошибка при парсинге.").AddValue(default(ImportStatementsResult)).AddInnerResult(helperResult);
            }

            await context.StatementPairs.AddRangeAsync(helperResult.Value.StatementPairs);
            ResultVocab dbSaveResult = await context.TrySaveChangesAsync();

            ImportStatementsResult importStatementsResult = new(helperResult.Value.StatementPairs.Count, helperResult.Value.FailedStatementPairs);
            return ResultVocab.Ok("Выражения импортированы успешно.").AddValue(importStatementsResult).AddInnerResult(dbSaveResult);
        }

        public async Task<ResultVocab<StatementDictionary>> Add(Guid userId, StatementDictionary dictionary)
        {
            userId.Throw().IfDefault();
            dictionary.ThrowIfNull();

            var valResult = new StatementDictionaryValidator(willBeInserted: true).Validate(dictionary);
            if (!valResult.IsValid)
            {
                return ResultVocab.Fail(valResult.ToString()).AddValue(default(StatementDictionary));
            }

            await context.StatementDictionaries.AddAsync(dictionary);
            return (await context.TrySaveChangesAsync(ResultMessages.Added)).AddValue(dictionary);
        }

        public async Task<ResultVocab> SetName(Guid userId, long dictionaryId, string name)
        {
            userId.Throw().IfDefault();
            dictionaryId.Throw().IfDefault();
            name.ThrowIfNull().IfEmpty().IfWhiteSpace();

            int rowsUpdated = await context.StatementDictionaries.Where(sd => sd.OwnerId == userId && sd.Id == dictionaryId).ExecuteUpdateAsync(sd => sd.SetProperty(p => p.Name, name));
            return rowsUpdated == 1 ? ResultVocab.Ok(ResultMessages.Updated) : ResultVocab.Fail(ResultMessages.UpdateError);
        }

        public async Task<ResultVocab> SetPositionPriority(Guid userId, long dictionaryId, int positionPriority)
        {
            userId.Throw().IfDefault();
            dictionaryId.Throw().IfDefault();

            int rowsUpdated = await context.StatementDictionaries.Where(sd => sd.OwnerId == userId && sd.Id == dictionaryId)
                .ExecuteUpdateAsync(sd => sd.SetProperty(p => p.PositionPriority, positionPriority));
            return rowsUpdated == 1 ? ResultVocab.Ok(ResultMessages.Updated) : ResultVocab.Fail(ResultMessages.UpdateError);
        }
    }
}

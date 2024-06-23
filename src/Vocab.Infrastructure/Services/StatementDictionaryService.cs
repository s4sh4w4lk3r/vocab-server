using Microsoft.EntityFrameworkCore;
using Throw;
using Vocab.Application.Abstractions.Services;
using Vocab.Application.Validators;
using Vocab.Application.ValueObjects.Result;
using Vocab.Application.ValueObjects.Result.Errors;
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

            int rowsDeleted = await context.StatementDictionaries
                .Where(sd => sd.Id == dictionaryId && sd.OwnerId == userId)
                .ExecuteDeleteAsync();

            return rowsDeleted == 1 ? ResultVocab.Success() : ResultVocab.Failure(StatementDictionaryErrors.NotFound);
        }

        public async Task<ResultVocab<long>> Add(Guid userId, string name)
        {
            const int MAX_NUMBER_OF_DICTIONARIES = 150;

            userId.Throw().IfDefault();
            name.ThrowIfNull().IfEmpty().IfWhiteSpace();

            if (await context.StatementDictionaries.CountAsync(x => x.OwnerId == userId) > MAX_NUMBER_OF_DICTIONARIES)
            {
                return ResultVocab.Failure(StatementDictionaryErrors.TooManyDictionaries).AddValue<long>(default);
            }

            StatementDictionary statementDictionary = new()
            {
                Id = default,
                LastModified = DateTime.UtcNow,
                Name = name,
                OwnerId = userId,
            };

            var valResult = new StatementDictionaryValidator(willBeInserted: true).Validate(statementDictionary);
            if (!valResult.IsValid)
            {
                return ResultVocab.Failure(StatementDictionaryErrors.Validation(valResult)).AddValue<long>(default);
            }


            await context.StatementDictionaries.AddAsync(statementDictionary);
            return (await context.TrySaveChangesAsync()).AddValue(statementDictionary.Id);
        }

        public async Task<ResultVocab> SetName(Guid userId, long dictionaryId, string name)
        {
            userId.Throw().IfDefault();
            dictionaryId.Throw().IfDefault();
            name.ThrowIfNull().IfEmpty().IfWhiteSpace();

            int rowsUpdated = await context.StatementDictionaries
                .Where(sd => sd.OwnerId == userId && sd.Id == dictionaryId)
                .ExecuteUpdateAsync(sd => sd
                    .SetProperty(p => p.Name, name)
                    .SetProperty(p => p.LastModified, DateTime.UtcNow));

            return rowsUpdated == 1 ? ResultVocab.Success() : ResultVocab.Failure(StatementDictionaryErrors.NotFound);
        }


        public async Task<ResultVocab<StatementDictionary>> GetById(Guid userId, long dictionaryId)
        {
            userId.Throw().IfDefault();
            dictionaryId.Throw().IfDefault();

            StatementDictionary? dictionary = await context.StatementDictionaries
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.Id == dictionaryId && x.OwnerId == userId);

            return dictionary is not null ? ResultVocab.Success().AddValue(dictionary) : ResultVocab.Failure(StatementDictionaryErrors.NotFound).AddValue<StatementDictionary>(default);
        }

        public async Task<ResultVocab<StatementDictionary[]>> GetUserDictionaries(Guid userId, bool appendStatements, int page)
        {
            userId.Throw().IfDefault();
            page.Throw().IfNegative();

            IQueryable<StatementDictionary> query = context.StatementDictionaries;
            if (appendStatements is true)
            {
                query = query.Include(x => x.StatementPairs.OrderBy(x => x.Source).Take(IStatementDictionaryService.NUMBER_OF_STATEMENTS_TO_INCLUDE));
            }

            StatementDictionary[] dictionaries = await query
                    .AsNoTracking()
                    .Where(x => x.OwnerId == userId)
                    .OrderByDescending(x => x.LastModified)
                    .Skip(IStatementDictionaryService.PAGE_SIZE * page)
                    .Take(IStatementDictionaryService.PAGE_SIZE)
                    .ToArrayAsync();

            return ResultVocab.Success().AddValue(dictionaries);
        }

        public async Task<ResultVocab<StatementDictionary[]>> SearchByName(Guid userId, string name, bool appendStatements, int page)
        {
            userId.Throw().IfDefault();
            page.Throw().IfNegative();

            IQueryable<StatementDictionary> query = context.StatementDictionaries;
            if (appendStatements is true)
            {
                query = query.Include(x => x.StatementPairs.OrderBy(x => x.Source).Take(IStatementDictionaryService.NUMBER_OF_STATEMENTS_TO_INCLUDE));
            }

            StatementDictionary[] dictionaries = await query
                    .AsNoTracking()
                    .Where(x => x.OwnerId == userId && x.Name.Contains(name))
                    .OrderByDescending(x => x.LastModified)
                    .Skip(IStatementDictionaryService.PAGE_SIZE * page)
                    .Take(IStatementDictionaryService.PAGE_SIZE)
                    .ToArrayAsync();

            return ResultVocab.Success().AddValue(dictionaries);
        }
    }
}

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

        public async Task<ResultVocab<StatementDictionary>> GetById(Guid userId, long dictionaryId)
        {
            userId.Throw().IfDefault();
            dictionaryId.Throw().IfDefault();

            StatementDictionary? statementDictionary = await context.StatementDictionaries
                .AsNoTracking()
                .SingleOrDefaultAsync(x=>x.Id == dictionaryId && x.OwnerId == userId);

            return statementDictionary is not null ? ResultVocab.Success().AddValue(statementDictionary) : ResultVocab.Failure(StatementDictionaryErrors.NotFound).AddValue<StatementDictionary>(default);
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
                statementDictionaries = await context.StatementDictionaries
                    .AsNoTracking().Where(x => x.OwnerId == userId)
                    .OrderBy(x => x.Name).Skip(offset).Take(NUMBER_OF_DICTIONARIES)
                    .Include(x=>x.StatementPairs.OrderBy(z => z.Source).Take(NUMBER_OF_TOP_STATEMENTS))
                    .ToArrayAsync();
            }
            else
            {
                statementDictionaries = await context.StatementDictionaries
                    .AsNoTracking()
                    .Where(x => x.OwnerId == userId)
                    .OrderBy(x=>x.Name).Skip(offset)
                    .Take(NUMBER_OF_DICTIONARIES)
                    .ToArrayAsync();
            }

            return ResultVocab.Success().AddValue(statementDictionaries);
        }

        public async Task<ResultVocab<long>> Add(Guid userId, string name)
        {
            userId.Throw().IfDefault();
            name.ThrowIfNull().IfEmpty().IfWhiteSpace();

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
                    .SetProperty(p=>p.LastModified, DateTime.UtcNow));

            return rowsUpdated == 1 ? ResultVocab.Success() : ResultVocab.Failure(StatementDictionaryErrors.NotFound);
        }
    }
}

using Vocab.Application.ValueObjects.Result;
using Vocab.Application.ValueObjects.Result.Errors;
using Vocab.Core.Entities;
using Vocab.Core.Enums;

namespace Vocab.Application.Abstractions.Services
{
    public interface IStatementPairService
    {
        public Task<ResultVocab<long>> Add(Guid userId, StatementPair statementPair);
        public Task<ResultVocab> Delete(Guid userId, long statementPairId);
        public Task<ResultVocab<StatementPair>> GetById(Guid userId, long statementPairId);
        public Task<ResultVocab<StatementPair[]>> GetStatements(Guid userId, long dictionaryId, int offset);

        public Task<ResultVocab> SetSource(Guid userId, long statementPairId, string source);
        public Task<ResultVocab> SetTarget(Guid userId, long statementPairId, string target);
        public Task<ResultVocab> SetCategory(Guid userId, long statementPairId, StatementCategory category);

        public async Task<ResultVocab> Patch(Guid userId, long statementPairId, string? source, string? target, StatementCategory category)
        {
            List<ResultVocab> results = new(3);

            if (category is not StatementCategory.None)
            {
                results.Add(await SetCategory(userId, statementPairId, category));
            }

            if (string.IsNullOrWhiteSpace(source) is false) 
            {
                results.Add(await SetSource(userId, statementPairId, source));
            }

            if (string.IsNullOrWhiteSpace(target) is false)
            {
                results.Add(await SetTarget(userId, statementPairId, target));
            }

            if (results.Count == 0)
            {
                return ResultVocab.Failure(StatementPairErrors.PatchNoAction);
            }

            if (results.Count != results.Count(x=>x.IsSuccess is true))
            {
                return results.First(x => x.IsSuccess is false);
            }

            return ResultVocab.Success();
        }
    }
}

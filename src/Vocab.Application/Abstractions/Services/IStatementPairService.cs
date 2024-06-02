using Vocab.Application.ValueObjects;
using Vocab.Core.Entities;
using Vocab.Core.Enums;

namespace Vocab.Application.Abstractions.Services
{
    public interface IStatementPairService
    {
        public Task<ResultVocab<StatementPair>> Add(Guid userId, StatementPair statementPair);
        public Task<ResultVocab> Delete(Guid userId, long statementPairId);
        public Task<ResultVocab<StatementPair>> GetById(Guid userId, long statementPairId);
        public Task<ResultVocab<StatementPair[]>> GetDictionaryStatementPairs(Guid userId, long dictionaryId, int offset);

        public Task<ResultVocab> SetSource(Guid userId, long statementPairId, string source);
        public Task<ResultVocab> SetTarget(Guid userId, long statementPairId, string target);
        public Task<ResultVocab> SetCategory(Guid userId, long statementPairId, StatementCategory category);
        public Task<ResultVocab> SetPositionPriority(Guid userId, long statementPairId, int positionPriority);
    }
}

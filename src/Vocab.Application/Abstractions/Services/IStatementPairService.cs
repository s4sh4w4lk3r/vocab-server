using Vocab.Application.ValueObjects;
using Vocab.Core.Entities;
using Vocab.Core.Enums;

namespace Vocab.Application.Abstractions.Services
{
    public interface IStatementPairService
    {
        public Task<ResultVocab<StatementPair>> Insert(Guid userId, StatementPair statementPair);
        public Task<ResultVocab<StatementPair>> Update(Guid userId, StatementPair statementPair);
        public Task<ResultVocab> Delete(Guid userId, long statementPairId);

        public Task<ResultVocab> SetSource(Guid userId, long statementPairId, string source);
        public Task<ResultVocab> SetTarget(Guid userId, long statementPairId, string target);
        public Task<ResultVocab> SetCategory(Guid userId, long statementPairId, StatementCategory category);
    }
}

using Vocab.Application.ValueObjects;
using Vocab.Core.Entities;
using Vocab.Core.Enums;

namespace Vocab.Application.Abstractions.Services
{
    public interface IStatementPairService : IRatingService
    {
        public Task<ResultVocab<StatementPair>> Insert(StatementPair statementPair);
        public Task<ResultVocab<StatementPair>> Update(StatementPair statementPair);
        public Task<ResultVocab> Delete(Guid userId, long id);

        public Task<ResultVocab<StatementPair>> SetSource(Guid userId, long statementPairId, string source);
        public Task<ResultVocab<StatementPair>> SetTarget(Guid userId, long statementPairId, string target);
        public Task<ResultVocab<StatementPair>> SetCategory(Guid userId, long statementPairId, StatementCategory category);
    }
}

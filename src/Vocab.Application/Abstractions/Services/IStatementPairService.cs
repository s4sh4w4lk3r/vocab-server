using Vocab.Application.ValueObjects;
using Vocab.Core.Entities;
using Vocab.Core.Enums;

namespace Vocab.Application.Abstractions.Services
{
    public interface IStatementPairService : IRatingService
    {
        public Task<Result<StatementPair>> Insert(StatementPair statementPair);
        public Task<Result<StatementPair>> Update(StatementPair statementPair);
        public Task<Result> Delete(long id);

        public Task<Result<StatementPair>> SetSource(long id, string source);
        public Task<Result<StatementPair>> SetTarget(long id, string target);
        public Task<Result<StatementPair>> SetCategory(long id, StatementCategory category);
    }
}

using Vocab.Application.ValueObjects;
using Vocab.Core.Entities;

namespace Vocab.Application.Abstractions.Services
{
    public interface IDictionaryService
    {
        public Task<Result<StatementDictionary>> Create(StatementDictionary dictionary);
        public Task<Result<StatementDictionary>> Update(StatementDictionary dictionary);
        public Task<Result> Delete(long id);

        public Task<Result<StatementDictionary>> SetName(long id, string name);
        public Task<Result<IQueryable<StatementPair>>> GetStatementsForChallenge(long dictionaryId);
    }   
}

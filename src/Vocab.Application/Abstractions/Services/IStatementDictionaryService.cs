using Vocab.Application.ValueObjects.Result;
using Vocab.Core.Entities;

namespace Vocab.Application.Abstractions.Services
{
    public interface IStatementDictionaryService
    {
        public Task<ResultVocab<long>> Add(Guid userId, string name);
        public Task<ResultVocab> Delete(Guid userId, long dictionaryId);
        public Task<ResultVocab<StatementDictionary>> GetById(Guid userId, long dictionaryId);
        public Task<ResultVocab<StatementDictionary[]>> GetUserDictionaries(Guid userId, bool appendTopStatements, int offset);

        public Task<ResultVocab> SetName(Guid userId, long dictionaryId, string name);

    }   
}

using Vocab.Application.Types;
using Vocab.Application.ValueObjects;
using Vocab.Core.Entities;

namespace Vocab.Application.Abstractions.Services
{
    public interface IStatementDictionaryService
    {
        public Task<ResultVocab<StatementDictionary>> Add(Guid userId, StatementDictionary dictionary);
        public Task<ResultVocab> Delete(Guid userId, long dictionaryId);
        public Task<ResultVocab<StatementDictionary>> GetById(Guid userId, long dictionaryId);
        public Task<ResultVocab<StatementDictionary[]>> GetUserDictionaries(Guid userId, bool appendTopStatements, int offset);

        public Task<ResultVocab> SetName(Guid userId, long dictionaryId, string name);
        public Task<ResultVocab<ImportStatementsResult>> ImportStatements(Guid userId, long dictionaryId, Stream stream, string separator);
        public Task<ResultVocab> SetPositionPriority(Guid userId, long dictionaryId, int positionPriority);
    }   
}

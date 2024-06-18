using Vocab.Application.Enums;
using Vocab.Application.ValueObjects.Result;
using Vocab.Core.Entities;

namespace Vocab.Application.Abstractions.Services
{
    public interface IStatementDictionaryService
    {
        public Task<ResultVocab<long>> Add(Guid userId, string name);
        public Task<ResultVocab> Delete(Guid userId, long dictionaryId);
        public Task<ResultVocab> SetName(Guid userId, long dictionaryId, string name);

        public Task<ResultVocab<StatementDictionary>> GetById(Guid userId, long dictionaryId, AppendStatementsAction appendAction);
        public Task<ResultVocab<StatementDictionary[]>> GetUserDictionaries(Guid userId, AppendStatementsAction appendAction, int offset);
        public Task<ResultVocab<StatementDictionary[]>> SearchByName(Guid userId, string name, AppendStatementsAction appendAction, int offset);

    }   
}

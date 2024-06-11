using Vocab.Application.ValueObjects.Result;

namespace Vocab.Application.Abstractions.Services
{
    public interface IStatementsImportService
    {
        public Task<ResultVocab<string>> ImportStatements(Guid userId, long dictionaryId, string documentBase64, string separator);
    }
}

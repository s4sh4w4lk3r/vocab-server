using Vocab.Application.ValueObjects;

namespace Vocab.Application.Abstractions.Services
{
    public interface IRatingService
    {
        public Task<ResultVocab<int>> HandleAnswer(Guid userId, long statementPairId, string userGuess);
    }
}

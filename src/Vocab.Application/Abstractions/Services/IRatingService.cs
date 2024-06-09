using Vocab.Application.Types;
using Vocab.Application.ValueObjects.Result;

namespace Vocab.Application.Abstractions.Services
{
    public interface IRatingService
    {
        public Task<ResultVocab<AnswerResult>> HandleAnswer(Guid userId, long statementPairId, string userGuess);
    }
}

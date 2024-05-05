using Microsoft.EntityFrameworkCore;
using Throw;
using Vocab.Application.Abstractions.Services;
using Vocab.Application.ValueObjects;
using Vocab.Core.Entities;
using Vocab.Infrastructure.Persistence;

namespace Vocab.Infrastructure.Services
{
    public class RatingService(VocabContext vocabContext) : IRatingService
    {
        public async Task<ResultVocab<int>> HandleAnswer(Guid userId, long statementPairId, string userGuess)
        {
            userId.Throw().IfDefault();
            statementPairId.Throw().IfDefault();
            userGuess.ThrowIfNull().IfEmpty().IfWhiteSpace();

            StatementPair? rightTagret = await vocabContext.StatementPairs
                .Where(x => x.Id == statementPairId && x.StatementsDictionary!.OwnerId == userId)
                .SingleOrDefaultAsync();

            if (rightTagret is null)
            {
                return ResultVocab.Fail("Словосочетание не найдено").AddValue(default(int));
            }

            if (string.Equals(userGuess, rightTagret.Target, StringComparison.InvariantCultureIgnoreCase))
            {
                rightTagret.IncreaseRating();
                return (await vocabContext.TrySaveChangesAsync()).AddValue(rightTagret.GuessingLevel);
            }
            else
            {
                rightTagret.DecreaseRating();
                return (await vocabContext.TrySaveChangesAsync()).AddValue(rightTagret.GuessingLevel);
            }
        }
    }
}

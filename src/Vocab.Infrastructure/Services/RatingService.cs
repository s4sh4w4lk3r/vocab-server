using Microsoft.EntityFrameworkCore;
using Throw;
using Vocab.Application.Abstractions.Services;
using Vocab.Application.Constants;
using Vocab.Application.Types;
using Vocab.Application.ValueObjects;
using Vocab.Core.Entities;
using Vocab.Infrastructure.Persistence;

namespace Vocab.Infrastructure.Services
{
    public class RatingService(VocabContext vocabContext) : IRatingService
    {
        public async Task<ResultVocab<AnswerResult>> HandleAnswer(Guid userId, long statementPairId, string userGuess)
        {
            userId.Throw().IfDefault();
            statementPairId.Throw().IfDefault();
            userGuess.ThrowIfNull().IfEmpty().IfWhiteSpace();

            StatementPair? rightTagret = await vocabContext.StatementPairs
                .Where(x => x.Id == statementPairId && x.StatementsDictionary!.OwnerId == userId)
                .SingleOrDefaultAsync();

            if (rightTagret is null)
            {
                return ResultVocab.Fail("Словосочетание не найдено").AddValue(default(AnswerResult));
            }

            int prevRating = rightTagret.GuessingLevel;

            AnswerResult answerResult = string.Equals(userGuess, rightTagret.Target, StringComparison.InvariantCultureIgnoreCase)
                ? new(CurrentRating: rightTagret.IncreaseRating(), IsAnswerRight: true) 
                : new(CurrentRating: rightTagret.DecreaseRating(), IsAnswerRight: false);


            if (prevRating != answerResult.CurrentRating)
            {
                var saveToDbResult = await vocabContext.TrySaveChangesAsync();
                return ResultVocab.Ok(ResultMessages.Updated).AddInnerResult(saveToDbResult).AddValue(answerResult);
            }
            else
            {
                return ResultVocab.Ok(ResultMessages.Updated).AddValue(answerResult);
            }
        }
    }
}

using Vocab.Application.ValueObjects;

namespace Vocab.Application.Abstractions.Services
{
    public interface IRatingService
    {
        /// <summary>
        /// Увеличить рейтинг выражения.
        /// </summary>
        /// <returns>Текущее значение рейтинга.</returns>
        public Task<ResultVocab<int>> Increase(Guid userId, long statementPairId);

        /// <summary>
        /// Уменьшить рейтинг выражения.
        /// </summary>
        /// <returns>Текущее значение рейтинга.</returns>
        public Task<ResultVocab<int>> Decrease(Guid userId, long statementPairId);
    }
}

using Vocab.Application.ValueObjects;

namespace Vocab.Application.Abstractions.Services
{
    public interface IRatingService
    {
        /// <summary>
        /// Увеличить рейтинг выражения.
        /// </summary>
        /// <returns>Текущее значение рейтинга.</returns>
        public Task<Result<int>> Increase();

        /// <summary>
        /// Уменьшить рейтинг выражения.
        /// </summary>
        /// <returns>Текущее значение рейтинга.</returns>
        public Task<Result<int>> Decrease();
    }
}

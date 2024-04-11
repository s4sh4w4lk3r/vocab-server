using Vocab.Core.Abstractions;
using Vocab.Core.Enums;

namespace Vocab.Core.Entities
{
    /// <summary>
    /// Включает в себя оригинал и перевод выражения.
    /// </summary>
    public class StatementPair : ILastModifiedEntity
    {
        public required long Id { get; init; }
        public required string Source { get; init; }
        public required string Target { get; init; }
        public required StatementCategory StatementCategory { get; init; }

        /// <summary>
        /// Уровень знания первода. Может принимать значение от 1 (вкл.) до 5 (вкл.).
        /// </summary>
        public int GuessingLevel { get; private set; } = MAX_GUESSING_LEVEL;
        public required DateTime LastModified { get; init; }

        public required long RelatedDictionaryId { get; init; }
        public StatementDictionary? StatementsDictionary { get; init; }

        public const int MIN_GUESSING_LEVEL = 1, MAX_GUESSING_LEVEL = 5;
        public int IncreaseRating() => GuessingLevel < MAX_GUESSING_LEVEL ? ++GuessingLevel : GuessingLevel;
        public int DecreaseRating() => GuessingLevel > MIN_GUESSING_LEVEL ? --GuessingLevel : GuessingLevel;
    }
}

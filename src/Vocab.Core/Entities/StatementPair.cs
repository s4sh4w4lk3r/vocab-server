using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Vocab.Core.Enums;

namespace Vocab.Core.Entities
{
    /// <summary>
    /// Включает в себя оригинал и перевод выражения.
    /// </summary>
    public class StatementPair(): IEntity
    {
        public required long Id { get; init; }
        public required string Source { get; init; }
        public required string Target { get; init; }
        public required StatementCategory StatementCategory { get; init; } = StatementCategory.None;

        /// <summary>
        /// Уровень знания перевода. Может принимать значение от 1 (вкл.) до 5 (вкл.).
        /// </summary>
        public int GuessingLevel { get; private set; } = MIN_GUESSING_LEVEL;
        public required DateTime LastModified { get; init; }
        public required int PositionPriority { get; init; }

        public required long StatementsDictionaryId { get; init; }
        [JsonIgnore] public StatementDictionary? StatementsDictionary { get; init; }


        public const int MIN_GUESSING_LEVEL = 1, MAX_GUESSING_LEVEL = 5;
        public int IncreaseRating() => GuessingLevel < MAX_GUESSING_LEVEL ? ++GuessingLevel : GuessingLevel;
        public int DecreaseRating() => GuessingLevel > MIN_GUESSING_LEVEL ? --GuessingLevel : GuessingLevel;

        [SetsRequiredMembers]
        public StatementPair(long id, string source, string target, StatementCategory statementCategory, int guessingLevel, DateTime lastModified, long statementsDictionaryId) : this()
        {
            Id = id;
            Source = source;
            Target = target;
            StatementCategory = statementCategory;
            GuessingLevel = guessingLevel;
            LastModified = lastModified;
            StatementsDictionaryId = statementsDictionaryId;
        }
    }
}

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
        public required DateTime LastModified { get; init; }

        public required long RelatedDictionaryId { get; init; }
        public StatementDictionary? StatementsDictionary { get; init; }

    }
}

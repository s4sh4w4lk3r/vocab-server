﻿using Vocab.Core.Abstractions;
using Vocab.Core.Enums;

namespace Vocab.Core.Entities
{
    /// <summary>
    /// Включает в себя оригинал и перевод выражения.
    /// </summary>
    public class StatementPair : ILastModifiedEntity
    {
        public required ulong Id { get; init; }
        public required string Source { get; init; }
        public required string Target { get; init; }
        public required StatementCategory StatementCategory { get; init; }
        public required DateTime LastModified { get; init; }

        public required ulong RelatedDictionaryId { get; init; }
        public required StatementDictionary StatementsDictionary { get; init; }

    }
}

using System.Collections.ObjectModel;
using Vocab.Core.Abstractions;

namespace Vocab.Core.Entities
{
    public class StatementDictionary : ILastModifiedEntity
    {
        public required ulong Id { get; init; }
        public required string Name { get; init; }
        public required Guid OwnerId { get; init; }
        public required DateTime LastModified { get; init; }

        public required Collection<StatementPair> StatementPairs { get; init; }
    }
}

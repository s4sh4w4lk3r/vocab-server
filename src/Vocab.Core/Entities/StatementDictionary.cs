using System.Collections.ObjectModel;

namespace Vocab.Core.Entities
{
    public class StatementDictionary: IEntity
    {
        public required long Id { get; init; }
        public required string Name { get; init; }
        public required Guid OwnerId { get; init; }
        public required DateTime LastModified { get; init; }

        public Collection<StatementPair>? StatementPairs { get; init; }
    }
}

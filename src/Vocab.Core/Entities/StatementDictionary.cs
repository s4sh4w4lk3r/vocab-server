using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Vocab.Core.Entities
{
    public class StatementDictionary(): IEntity
    {
        public required long Id { get; init; }
        public required string Name { get; init; }
        public required Guid OwnerId { get; init; }
        public required DateTime LastModified { get; init; }

        [JsonIgnore] public ICollection<StatementPair> StatementPairs => new HashSet<StatementPair>();

        [SetsRequiredMembers]
        public StatementDictionary(long id, string name, Guid ownerId, DateTime lastModified) : this()
        {
            Id = id;
            Name = name;
            OwnerId = ownerId;
            LastModified = lastModified;
        }
    }
}

using Vocab.Core.Entities;
using Vocab.Core.Enums;

namespace Vocab.WebApi.Models
{
    public class StatementPairDto
    {
        public required string Source { get; init; }
        public required string Target { get; init; }
        public required StatementCategory StatementCategory { get; init; }
        public required long RelatedDictionaryId { get; init; }

        public StatementPair ToEntity()
        {
            return new()
            {
                Id = 0,
                Source = Source,
                Target = Target,
                StatementCategory = StatementCategory,
                StatementsDictionaryId = RelatedDictionaryId,
                LastModified = DateTime.UtcNow,
                PositionPriority = 0
            };
        }
    }
}

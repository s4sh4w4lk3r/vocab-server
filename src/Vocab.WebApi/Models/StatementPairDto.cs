using System.ComponentModel.DataAnnotations;
using Vocab.Core.Entities;
using Vocab.Core.Enums;

namespace Vocab.WebApi.Models
{
    public class StatementPairDto
    {
        [StringLength(StatementPair.MAX_SOURCE_LENGTH)]
        public required string Source { get; init; }

        [StringLength(StatementPair.MAX_TARGET_LENGTH)]
        public required string Target { get; init; }

        public required StatementCategory StatementCategory { get; init; }

        [Range(1, long.MaxValue)]
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
            };
        }
    }
}

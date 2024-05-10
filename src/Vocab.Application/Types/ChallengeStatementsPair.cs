using System.Diagnostics.CodeAnalysis;

namespace Vocab.Application.Types
{
    public class ChallengeStatementsPair
    {

        public required long StatementsPairId { get; init; }
        public required string Source { get; init; }
        public required string Target1 { get; init; }
        public required string Target2 { get; init; }

        [SetsRequiredMembers]
        public ChallengeStatementsPair(long statementsPairId, string source, string target1, string target2)
        {
            StatementsPairId = statementsPairId;
            Source = source;
            Target1 = target1;
            Target2 = target2;
        }
    }
}

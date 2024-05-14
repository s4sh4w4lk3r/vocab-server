namespace Vocab.Application.Types
{
    public class ImportStatementsResult(int successfulAddictionsCount, IEnumerable<string> unsuccessfulLines)
    {
        public int SuccessfulAddictionsCount { get; init; } = successfulAddictionsCount;
        public IEnumerable<string> UnsuccessfulLines { get; init; } = unsuccessfulLines;
    }
}

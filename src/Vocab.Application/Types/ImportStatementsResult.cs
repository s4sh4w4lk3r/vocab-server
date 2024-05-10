namespace Vocab.Application.Types
{
    public class ImportStatementsResult(int successfulAddictionsCount, List<string> unsuccessfulLines)
    {
        public int SuccessfulAddictionsCount { get; init; } = successfulAddictionsCount;
        public List<string> UnsuccessfulLines { get; init; } = unsuccessfulLines;
    }
}

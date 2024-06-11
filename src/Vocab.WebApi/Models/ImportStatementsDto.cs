namespace Vocab.WebApi.Models
{
    public class ImportStatementsDto
    {
        public required string DocumentBase64 { get; init; }
        public string Separator { get; init; } = " - ";
    }
}

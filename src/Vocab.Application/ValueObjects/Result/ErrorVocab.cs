namespace Vocab.Application.ValueObjects.ErrorResult
{
    public record ErrorVocab(string Code, string Description, ErrorType ErrorType)
    {
        public static readonly ErrorVocab None = new(string.Empty, string.Empty, ErrorType.Failure);
        public static readonly ErrorVocab NullValue = new("Error.NullValue", "Получен Null", ErrorType.Failure);

        public static ErrorVocab Failure(string code, string description) =>
            new(code, description, ErrorType.Failure);
        public static ErrorVocab Validation(string code, string description) =>
            new(code, description, ErrorType.Validation);
        public static ErrorVocab NotFound(string code, string description) =>
            new(code, description, ErrorType.NotFound);
        public static ErrorVocab Conflict(string code, string description) =>
            new(code, description, ErrorType.Conflict);

    }
}

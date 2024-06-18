using Vocab.Application.Enums;

namespace Vocab.Application.ValueObjects.Result
{
    public record ErrorVocab(string Code, string Description, ErrorType ErrorType)
    {
        public static readonly ErrorVocab None = new(string.Empty, string.Empty, ErrorType.Failure);
        public static readonly ErrorVocab NullValue = new("Error.NullValue", "Получен Null", ErrorType.Failure);

        internal static ErrorVocab Failure(string code, string description) =>
            new(code, description, ErrorType.Failure);
        internal static ErrorVocab Validation(string code, string description) =>
            new(code, description, ErrorType.Validation);
        internal static ErrorVocab NotFound(string code, string description) =>
            new(code, description, ErrorType.NotFound);
        internal static ErrorVocab Conflict(string code, string description) =>
            new(code, description, ErrorType.Conflict);

    }
}

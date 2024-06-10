using FluentValidation.Results;

namespace Vocab.Application.ValueObjects.Result.Errors
{
    public class StatementDictionaryErrors
    {
        public static readonly ErrorVocab NotFound = ErrorVocab.NotFound("StatementDictionary.NotFound", "Словарь не найден, либо не принадлежит данному пользователю.");
        public static ErrorVocab Validation(ValidationResult validationResult) => ErrorVocab.Validation("StatementDictionary.Validation", validationResult.ToString());
        public static ErrorVocab EmptyLinesReceived = ErrorVocab.Validation("StatementDictionary.EmptyLinesReceived", "Ни одной строки не получено.");
        public static ErrorVocab NoExpressionImported = ErrorVocab.Failure("StatementDictionary.NoExpressionImported", "Ни одно выражение не импортировано.");

    }
}

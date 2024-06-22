using FluentValidation.Results;

namespace Vocab.Application.ValueObjects.Result.Errors
{
    public static class StatementPairErrors
    {
        public static readonly ErrorVocab NotFound = ErrorVocab.NotFound("StatementPair.NotFound.", "Выражение не найдено.");
        public static ErrorVocab Validation(ValidationResult validationResult) => ErrorVocab.Validation("StatementPair.Validation", validationResult.ToString());
        public static readonly ErrorVocab TooManyStatements = ErrorVocab.Conflict("StatementPair.TooManyStatements", "Достигнут лимит по количеству выражений в словаре.");
        public static readonly ErrorVocab PatchNoAction = ErrorVocab.Failure("StatementPair.PatchNoAction", "Не получено ни одного значения для обновления выражения.");
    }
}

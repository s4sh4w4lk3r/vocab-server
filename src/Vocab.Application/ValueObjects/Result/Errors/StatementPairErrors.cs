using FluentValidation.Results;

namespace Vocab.Application.ValueObjects.Result.Errors
{
    public static class StatementPairErrors
    {
        public static readonly ErrorVocab NotFound = ErrorVocab.NotFound("StatementPair.NotFound.", "Выражение не найдено.");
        public static ErrorVocab Validation(ValidationResult validationResult) => ErrorVocab.Validation("StatementPair.Validation", validationResult.ToString());
    }
}

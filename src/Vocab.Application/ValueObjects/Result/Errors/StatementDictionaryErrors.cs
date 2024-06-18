using FluentValidation.Results;

namespace Vocab.Application.ValueObjects.Result.Errors
{
    public class StatementDictionaryErrors
    {
        public static readonly ErrorVocab NotFound = ErrorVocab.NotFound("StatementDictionary.NotFound", "Словарь не найден, либо не принадлежит данному пользователю.");
        public static ErrorVocab Validation(ValidationResult validationResult) => ErrorVocab.Validation("StatementDictionary.Validation", validationResult.ToString());
        public static readonly ErrorVocab TooManyDictionaries = ErrorVocab.Conflict("StatementDictionary.TooManyDictionaries", "Достигнут лимит по количеству словарей.");

    }
}

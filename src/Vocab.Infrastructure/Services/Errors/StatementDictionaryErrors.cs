using FluentValidation.Results;
using Vocab.Application.ValueObjects.ErrorResult;

namespace Vocab.Infrastructure.Services.Errors
{
    internal class StatementDictionaryErrors
    {
        public static readonly ErrorVocab NotFound = ErrorVocab.NotFound("StatementDictionary.NotFound", "Словарь не найден, либо не принадлежит данному пользователю.");
        public static ErrorVocab Validation(ValidationResult validationResult) => ErrorVocab.Validation("StatementDictionary.Validation", validationResult.ToString());
    }
}

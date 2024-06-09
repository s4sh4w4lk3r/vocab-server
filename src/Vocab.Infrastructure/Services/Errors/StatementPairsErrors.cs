using FluentValidation.Results;
using Vocab.Application.ValueObjects.ErrorResult;

namespace Vocab.Infrastructure.Services.Errors
{
    internal static class StatementPairsErrors
    {
        public static readonly ErrorVocab NotFound = ErrorVocab.NotFound("StatementPair.NotFound.", "Выражение не найдено, либо не принадлежит данному пользователю.");
        public static ErrorVocab Validation(ValidationResult validationResult) => ErrorVocab.Validation("StatementPair.Validation", validationResult.ToString());
    }
}

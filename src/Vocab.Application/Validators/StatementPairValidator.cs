using FluentValidation;
using Vocab.Core.Entities;

namespace Vocab.Application.Validators
{
    public class StatementPairValidator : AbstractValidator<StatementPair>
    {
        public StatementPairValidator(bool isIdValidationRequired)
        {
            _ = isIdValidationRequired ? RuleFor(sp => sp.Id).NotEmpty() : null;

            RuleFor(sp => sp.RelatedDictionaryId).NotEmpty();
            RuleFor(sp => sp.Source).NotEmpty();
            RuleFor(sp => sp.Target).NotEmpty();
            RuleFor(sp => sp.StatementCategory).IsInEnum();
            RuleFor(sp => sp.LastModified).NotEmpty();
            RuleFor(sp => sp.GuessingLevel).GreaterThanOrEqualTo(StatementPair.MIN_GUESSING_LEVEL).LessThanOrEqualTo(StatementPair.MAX_GUESSING_LEVEL);

            RuleFor(sp => sp.StatementsDictionary).Null();
        }
    }
}

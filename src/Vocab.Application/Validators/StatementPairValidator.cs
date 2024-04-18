using FluentValidation;
using Vocab.Core.Entities;

namespace Vocab.Application.Validators
{
    public class StatementPairValidator : AbstractValidator<StatementPair>
    {
        public StatementPairValidator(bool willBeInserted)
        {
            _ = willBeInserted ? RuleFor(sp => sp.Id).Empty() : RuleFor(sp => sp.Id).NotEmpty();

            RuleFor(sp => sp.RelatedDictionaryId).NotEmpty();
            RuleFor(sp => sp.Source).NotEmpty();
            RuleFor(sp => sp.Target).NotEmpty();
            RuleFor(sp => sp.StatementCategory).IsInEnum();
            RuleFor(sp => sp.LastModified).NotEmpty().Must(dt => dt.Kind == DateTimeKind.Utc);
            RuleFor(sp => sp.GuessingLevel).GreaterThanOrEqualTo(StatementPair.MIN_GUESSING_LEVEL).LessThanOrEqualTo(StatementPair.MAX_GUESSING_LEVEL);

            RuleFor(sp => sp.StatementsDictionary).Null();
        }
    }
}

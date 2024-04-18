using FluentValidation;
using Vocab.Core.Entities;

namespace Vocab.Application.Validators
{
    public class StatementDictionaryValidator : AbstractValidator<StatementDictionary>
    {
        public StatementDictionaryValidator(bool willBeInserted)
        {
            _ = willBeInserted ? RuleFor(sd => sd.Id).Empty() : RuleFor(sd => sd.Id).NotEmpty();

            RuleFor(sd => sd.Name).NotEmpty();
            RuleFor(sd => sd.OwnerId).NotEmpty();
            RuleFor(sd => sd.LastModified).NotEmpty().Must(dt=>dt.Kind == DateTimeKind.Utc);

            RuleFor(sd => sd.StatementPairs).Null();
        }
    }
}

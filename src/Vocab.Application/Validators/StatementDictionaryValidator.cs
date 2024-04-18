using FluentValidation;
using Vocab.Core.Entities;

namespace Vocab.Application.Validators
{
    public class StatementDictionaryValidator : AbstractValidator<StatementDictionary>
    {
        public StatementDictionaryValidator(bool isIdValidationRequired)
        {
            _ = isIdValidationRequired ? RuleFor(sd => sd.Id).NotEmpty() : null;

            RuleFor(sd => sd.Name).NotEmpty();
            RuleFor(sd => sd.OwnerId).NotEmpty();
            RuleFor(sd => sd.LastModified).NotEmpty();

            RuleFor(sd => sd.StatementPairs).Null();
        }
    }
}

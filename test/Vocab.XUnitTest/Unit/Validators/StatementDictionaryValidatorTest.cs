using Vocab.Application.Validators;
using Vocab.Core.Entities;

namespace Vocab.XUnitTest.Unit.Validators
{
    public class StatementDictionaryValidatorTest
    {
        private readonly long validId = 1234;
        private readonly string validName = "Dictionary";
        private readonly Guid validOwnerId = Guid.NewGuid();
        private readonly DateTime validDateTime = DateTime.Now;

        [Theory]
        [InlineData(true, 1234, true)]
        [InlineData(true, 0, false)]
        [InlineData(true, 1234, false)]
        public void TestIdRule(bool expected, long id, bool isPKeyValidationRequired)
        {
            StatementDictionary statementDictionary = new(id, validName, validOwnerId, validDateTime);

            bool isValid = new StatementDictionaryValidator(isPKeyValidationRequired).Validate(statementDictionary).IsValid;

            Assert.Equal(expected, isValid);
        }

        [Theory]
        [InlineData(true, "Dictionary")]
        [InlineData(false, " ")]
        [InlineData(false, "")]
        public void TestStringRule(bool expected, string name)
        {
            StatementDictionary statementDictionary = new(validId, name, validOwnerId, validDateTime);

            bool isValid = new StatementDictionaryValidator(true).Validate(statementDictionary).IsValid;

            Assert.Equal(expected, isValid);
        }

        [Fact]
        public void TestEmptyGuidRule()
        {
            bool expected = false;
            StatementDictionary statementDictionary = new(validId, validName, Guid.Empty, validDateTime);

            bool isValid = new StatementDictionaryValidator(true).Validate(statementDictionary).IsValid;

            Assert.Equal(expected, isValid);
        }
    }
}

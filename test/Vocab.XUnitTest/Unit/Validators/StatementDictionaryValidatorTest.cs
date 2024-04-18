using FluentValidation;
using Vocab.Application.Validators;
using Vocab.Core.Entities;

namespace Vocab.XUnitTest.Unit.Validators
{
    public class StatementDictionaryValidatorTest
    {
        private readonly long validPKey = 1234;
        private readonly string validName = "Dictionary";
        private readonly Guid validFKey = Guid.NewGuid();
        private readonly DateTime validDateTime = DateTime.Now;
        private readonly bool validWillBeInserted = false;

        [Theory]
        [InlineData(true, 0, true)]
        [InlineData(true, 1234, false)]
        [InlineData(false, 1234, true)]
        [InlineData(false, 0, false)]
        public void TestPKeyRule(bool expected, long id, bool willBeInserted)
        {
            StatementDictionary statementDictionary = new(id, validName, validFKey, validDateTime);

            bool isValid = new StatementDictionaryValidator(willBeInserted).Validate(statementDictionary).IsValid;

            Assert.Equal(expected, isValid);
        }

        [Theory]
        [InlineData(true, "Dictionary")]
        [InlineData(false, " ")]
        [InlineData(false, "")]
        public void TestNameRule(bool expected, string name)
        {
            StatementDictionary statementDictionary = new(validPKey, name, validFKey, validDateTime);

            bool isValid = new StatementDictionaryValidator(validWillBeInserted).Validate(statementDictionary).IsValid;
            Assert.Equal(expected, isValid);
        }

        [Fact]
        public void TestEmptyOwnerGuidRule()
        {
            bool expected = false;
            StatementDictionary statementDictionary = new(validPKey, validName, Guid.Empty, validDateTime);

            bool isValid = new StatementDictionaryValidator(validWillBeInserted).Validate(statementDictionary).IsValid;

            Assert.Equal(expected, isValid);
        }
    }
}

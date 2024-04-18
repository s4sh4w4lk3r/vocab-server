using Vocab.Application.Validators;
using Vocab.Core.Entities;
using Vocab.Core.Enums;

namespace Vocab.XUnitTest.Unit.Validators
{
    public class StatementPairValidatorTest
    {
        [Theory]
        [InlineData(true, 1234, 1234, true)]
        [InlineData(true, 0, 1234, false)]
        [InlineData(false, 0, 1234, true)]
        [InlineData(false, 1234, 0, false)]
        public void TestIdRules(bool expected, long id, int relatedDictionaryId, bool isPKeyValidationRequired)
        {
            StatementPair statementPair = new(id, "Source", "Target", StatementCategory.None, 1, DateTime.Now, relatedDictionaryId);

            bool isValid = new StatementPairValidator(isPKeyValidationRequired).Validate(statementPair).IsValid;

            Assert.Equal(expected, isValid);
        }

        [Theory]
        [InlineData(true, "Source", "Target")]
        [InlineData(false, "Source", "")]
        [InlineData(false, "", "Target")]
        public void TestStringRules(bool expected, string source, string target)
        {
            StatementPair statementPair = new(1234, source, target, StatementCategory.None, 1, DateTime.Now, 1234);

            bool isValid = new StatementPairValidator(true).Validate(statementPair).IsValid;

            Assert.Equal(expected, isValid);
        }

        [Theory]
        [InlineData(true, StatementCategory.None)]
        [InlineData(false, (StatementCategory)111)]
        public void TestIsInEnumRule(bool expected, StatementCategory statementCategory)
        {
            StatementPair statementPair = new(1, "Source", "Target", statementCategory, 1, DateTime.Now, 1234);

            bool isValid = new StatementPairValidator(true).Validate(statementPair).IsValid;

            Assert.Equal(expected, isValid);
        }

        [Theory]
        [InlineData(true, 1)]
        [InlineData(true, 3)]
        [InlineData(false, 0)]
        [InlineData(false, 6)]
        public void TestGuessingLevelBoundaries(bool expected, int level)
        {
            StatementPair statementPair = new(1, "Source", "Target", StatementCategory.None, level, DateTime.Now, 1234);

            bool isValid = new StatementPairValidator(true).Validate(statementPair).IsValid;

            Assert.Equal(expected, isValid);
        }

        [Fact]
        public void TestNullValue()
        {
            StatementDictionary statementDictionary = new() { Id = 1, LastModified = DateTime.Now, Name = "Test", OwnerId = Guid.Empty };
            StatementPair statementPair = new(1, "Source", "Target", StatementCategory.None, 1, DateTime.Now, 1234) { StatementsDictionary = statementDictionary };

            bool isValid = new StatementPairValidator(true).Validate(statementPair).IsValid;

            Assert.False(isValid);
        }
    }
}
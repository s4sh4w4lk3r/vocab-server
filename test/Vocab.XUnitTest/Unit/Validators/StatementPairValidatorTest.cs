using Vocab.Application.Validators;
using Vocab.Core.Entities;
using Vocab.Core.Enums;

namespace Vocab.XUnitTest.Unit.Validators
{
    public class StatementPairValidatorTest
    {
        private readonly int validPKey = 1234;
        private readonly string validSource = "Source";
        private readonly string validTarget = "Target";
        private readonly StatementCategory validCategory = StatementCategory.None;
        private readonly int validLevel = 1;
        private readonly DateTime validDateTime = DateTime.Now;
        private readonly int validRelatedDictionaryFKey = 1234;
        private readonly bool validWillBeInserted = false;

        [Theory]
        [InlineData(true, 0, true)]
        [InlineData(true, 1234, false)]
        [InlineData(false, 1234, true)]
        [InlineData(false, 0, false)]
        public void TestPKeyRule(bool expected, long pkey, bool willBeInserted)
        {
            StatementPair statementPair = new(pkey, validSource, validTarget, validCategory, 1, validDateTime, validRelatedDictionaryFKey);

            bool isValid = new StatementPairValidator(willBeInserted).Validate(statementPair).IsValid;

            Assert.Equal(expected, isValid);
        }

        [Theory]
        [InlineData(true, 1234)]
        [InlineData(false, 0)]
        public void TestRelatedDictionaryFKRule(bool expected, long fkey)
        {
            StatementPair statementPair = new(validPKey, validSource, validTarget, validCategory, 1, validDateTime, fkey);

            bool isValid = new StatementPairValidator(validWillBeInserted).Validate(statementPair).IsValid;

            Assert.Equal(expected, isValid);
        }

        [Theory]
        [InlineData(true, "Source", "Target")]
        [InlineData(false, "Source", "")]
        [InlineData(false, "", "Target")]
        public void TestStringRules(bool expected, string source, string target)
        {
            StatementPair statementPair = new(1234, source, target, validCategory, 1, validDateTime, validRelatedDictionaryFKey);

            bool isValid = new StatementPairValidator(validWillBeInserted).Validate(statementPair).IsValid;

            Assert.Equal(expected, isValid);
        }

        [Theory]
        [InlineData(true, StatementCategory.None)]
        [InlineData(false, (StatementCategory)111)]
        public void TestIsInEnumRule(bool expected, StatementCategory statementCategory)
        {
            StatementPair statementPair = new(1, validSource, validTarget, statementCategory, 1, validDateTime, validRelatedDictionaryFKey);

            bool isValid = new StatementPairValidator(validWillBeInserted).Validate(statementPair).IsValid;

            Assert.Equal(expected, isValid);
        }

        [Theory]
        [InlineData(true, 1)]
        [InlineData(true, 3)]
        [InlineData(false, 0)]
        [InlineData(false, 6)]
        public void TestGuessingLevelBoundaries(bool expected, int level)
        {
            StatementPair statementPair = new(1, validSource, validTarget, validCategory, level, validDateTime, validRelatedDictionaryFKey);

            bool isValid = new StatementPairValidator(validWillBeInserted).Validate(statementPair).IsValid;

            Assert.Equal(expected, isValid);
        }

        [Fact]
        public void TestNullValue()
        {
            StatementDictionary statementDictionary = new() { Id = validRelatedDictionaryFKey, LastModified = validDateTime, Name = "Test", OwnerId = Guid.Empty };
            StatementPair statementPair = new(validPKey, validSource, validTarget, validCategory, validLevel, validDateTime, validRelatedDictionaryFKey) { StatementsDictionary = statementDictionary };

            bool isValid = new StatementPairValidator(validWillBeInserted).Validate(statementPair).IsValid;

            Assert.False(isValid);
        }
    }
}
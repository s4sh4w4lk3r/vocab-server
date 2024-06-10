namespace Vocab.Application.ValueObjects.Result.Errors
{
    public class ChallengeErrors
    {
        public static ErrorVocab InvalidStatementsCount(int minWords, int maxWords) =>
            ErrorVocab.Conflict("Challenge.InvalidStatementsCount", $"Количество слов для игры должно быть не меньше {minWords} и не более {maxWords}.");

        public static ErrorVocab TooFewStatements(int count, int minRequired) =>
            ErrorVocab.Conflict("Challenge.TooFewStatements", $"В словаре недостаточно слов для игры. Слов: {count}, требуется: {minRequired}.");

        public static readonly ErrorVocab ResponseTimeout = ErrorVocab.Conflict("Challenge.ResponseTimeout", "Время ожидания ответа вышло.");
    }
}

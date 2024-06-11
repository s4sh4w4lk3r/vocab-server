namespace Vocab.Application.ValueObjects.Result.Errors
{
    public class ImportStatementsErrors
    {
        public static readonly ErrorVocab EmptyLinesReceived = ErrorVocab.Validation("ImportDictionary.EmptyLinesReceived", "Ни одной строки не получено.");
        public static readonly ErrorVocab NoExpressionImported = ErrorVocab.Failure("ImportDictionary.NoExpressionImported", "Ни одно выражение не импортировано.");
        public static readonly ErrorVocab NotFound = ErrorVocab.NotFound("ImportDictionary.NotFound", StatementDictionaryErrors.NotFound.Description);
        public static readonly ErrorVocab Base64ToUtf8Error = ErrorVocab.Failure("ImportDictionary.Base64ToUtf8Error", "Не получилось преобразовать base64 в utf8.");
    }
}

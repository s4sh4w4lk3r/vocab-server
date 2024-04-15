namespace Vocab.Application.ValueObjects
{
    public class ResultVocab(bool success, string description)
    {
        public string Description { get; init; } = description;
        public bool Success { get; init; } = success;
        public ResultVocab? InnerResult { get; protected set; }

        public static ResultVocab Fail(string description) => new(false, description);
        public static ResultVocab Ok(string description = "") => new(true, description);

        public ResultVocab<TValue> AddValue<TValue>(TValue value)
        {
            var result = new ResultVocab<TValue>(Success, Description, value);
            return InnerResult is not null ? result.AddInnerResult(InnerResult) : result;
        }

        public ResultVocab AddInnerResult(ResultVocab innerResult)
        {
            InnerResult = innerResult;
            return this;
        }

        public override string ToString()
        {
            string innerResultMessage = InnerResult is not null ? $", InnerResult: {InnerResult}" : string.Empty;
            return $"Success: {Success}, Description: {Description}{innerResultMessage}";
        }
    }

    public class ResultVocab<TValue>(bool success, string description, TValue value) : ResultVocab(success, description)
    {
        public TValue Value { get; init; } = value;

        public new ResultVocab<TValue> AddInnerResult(ResultVocab innerResult)
        {
            InnerResult = innerResult;
            return this;
        }

        public override string ToString() => $"{base.ToString()}, Value: {Value}";
    }
}

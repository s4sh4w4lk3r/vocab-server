namespace Vocab.Application.ValueObjects
{
    public class Result(bool success, string description)
    {
        public string Description { get; init; } = description;
        public bool Success { get; init; } = success;
        public Result? InnerResult { get; protected set; }

        public static Result Fail(string description) => new(false, description);
        public static Result Ok(string description) => new(true, description);

        public Result<TValue> AddValue<TValue>(TValue value)
        {
            var result = new Result<TValue>(Success, Description, value);
            return InnerResult is not null ? result.AddInnerResult(InnerResult) : result;
        }

        public Result AddInnerResult(Result innerResult)
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

    public class Result<TValue>(bool success, string description, TValue value) : Result(success, description)
    {
        public TValue Value { get; init; } = value;

        public new Result<TValue> AddInnerResult(Result innerResult)
        {
            InnerResult = innerResult;
            return this;
        }

        public override string ToString() => $"{base.ToString()}, Value: {Value}";
    }
}

namespace Vocab.Application.ValueObjects.Result
{
    public class ResultVocab
    {
        public bool IsSuccess { get; init; }
        public ErrorVocab Error { get; init; } = ErrorVocab.None;


        public static ResultVocab Success()
        {
            return new ResultVocab
            {
                IsSuccess = true,
            };
        }
        public static ResultVocab Failure(ErrorVocab error)
        {
            return new ResultVocab
            {
                IsSuccess = false,
                Error = error
            };
        }

        public ResultVocab<TValue> AddValue<TValue>(TValue? value)
        {
            return new ResultVocab<TValue>
            {
                IsSuccess = this.IsSuccess,
                Value = value
            };
        }

    }

    public class ResultVocab<TValue> : ResultVocab
    {
        public TValue? Value { get; init; }
    }
}

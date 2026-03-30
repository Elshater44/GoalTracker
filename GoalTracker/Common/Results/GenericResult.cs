using GoalTracker.Common.Errors;

namespace GoalTracker.Common.Results
{
    public record Result<T> : Result
    {
        public T? Value { get; }

        private Result(T value) : base(true, null) =>
            Value = value ?? throw new ArgumentNullException(nameof(value));

        private Result(Error error) : base(false, error) => Value = default;
        public static Result<T> Success(T value) => new(value);
        public new static Result<T> Failure(Error error) => new(error);
    }
}

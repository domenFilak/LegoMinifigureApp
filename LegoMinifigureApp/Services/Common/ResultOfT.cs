namespace LegoMinifigureApp.Services.Common
{
    public sealed class Result<T>
    {
        public T? Value { get; }
        public Error? Error { get; }

        public bool IsSuccess => Error is null;
        public bool IsFailure => Error is not null;

        private Result(T? value, Error? error)
        {
            Value = value;
            Error = error;
        }

        public static Result<T> Success(T value)
        {
            return new Result<T>(value, null);
        }

        public static Result<T> Failure(Error error)
        {
            return new Result<T>(default, error);
        }
    }
}

namespace LegoMinifigureApp.Services.Common
{
    public sealed class Result
    {
        public Error? Error { get; }

        public bool IsSuccess => Error is null;
        public bool IsFailure => Error is not null;

        private Result(Error? error)
        {
            Error = error;
        }

        public static Result Success()
        {
            return new Result(null);
        }
        public static Result Failure(Error error)
        {
            return new Result(error);
        }
    }
}

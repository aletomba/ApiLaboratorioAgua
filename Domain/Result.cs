namespace Dominio
{
    public class Result
    {
        public bool IsSuccess => Error == null;
        public bool IsFailure => !IsSuccess;
        public string? Error { get; }

        private Result(string? error) { Error = error; }

        public static Result Success() => new(null);
        public static Result Failure(string error) => new(error);
    }

    public class Result<T>
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public T? Value { get; }
        public string? Error { get; }

        private Result(T value) { IsSuccess = true; Value = value; Error = null; }
        private Result(string error) { IsSuccess = false; Value = default; Error = error; }

        public static Result<T> Success(T value) => new(value);
        public static Result<T> Failure(string error) => new(error);
    }
}
